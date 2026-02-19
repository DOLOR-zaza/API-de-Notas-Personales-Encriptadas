// Vistas
const authView = document.getElementById("authView");
const appView = document.getElementById("appView");

// Top bar
const logoutBtn = document.getElementById("logoutBtn");
const userPill = document.getElementById("userPill");

// Auth
const tabLogin = document.getElementById("tabLogin");
const tabRegister = document.getElementById("tabRegister");
const authForm = document.getElementById("authForm");
const authSubmit = document.getElementById("authSubmit");
const registerExtra = document.getElementById("registerExtra");
const emailEl = document.getElementById("email");
const passwordEl = document.getElementById("password");
const nameEl = document.getElementById("name");
const togglePw = document.getElementById("togglePw");

// Notas UI
const notesList = document.getElementById("notesList");
const notesMeta = document.getElementById("notesMeta");
const searchInput = document.getElementById("searchInput");
const newNoteBtn = document.getElementById("newNoteBtn");

const noteForm = document.getElementById("noteForm");
const noteIdEl = document.getElementById("noteId");
const noteTitleEl = document.getElementById("noteTitle");
const noteContentEl = document.getElementById("noteContent");
const saveBtn = document.getElementById("saveBtn");
const clearBtn = document.getElementById("clearBtn");
const deleteBtn = document.getElementById("deleteBtn");
const lastSaved = document.getElementById("lastSaved");

// Toast
const toast = document.getElementById("toast");

// Modal
const modal = document.getElementById("modal");
const modalCancel = document.getElementById("modalCancel");
const modalConfirm = document.getElementById("modalConfirm");

let mode = "login"; // login | register
let notes = [];
let selectedId = null;
let pendingDeleteId = null;

let toastTimer = null;

function showToast(msg, ms = 5500) {
  toast.textContent = msg;
  toast.classList.remove("hidden");

  if (toastTimer) clearTimeout(toastTimer);
  toastTimer = setTimeout(() => toast.classList.add("hidden"), ms);
}

toast.addEventListener("click", () => toast.classList.add("hidden"));


function setAuthMode(next) {
  mode = next;
  tabLogin.classList.toggle("active", mode === "login");
  tabRegister.classList.toggle("active", mode === "register");
  registerExtra.classList.toggle("hidden", mode !== "register");
  authSubmit.textContent = mode === "login" ? "Entrar" : "Crear cuenta";
}

function showApp(email = "") {
  authView.classList.add("hidden");
  appView.classList.remove("hidden");
  logoutBtn.classList.remove("hidden");
  userPill.classList.remove("hidden");
  userPill.textContent = email ? `👤 ${email}` : "👤 Sesión activa";
}

function showAuth() {
  appView.classList.add("hidden");
  authView.classList.remove("hidden");
  logoutBtn.classList.add("hidden");
  userPill.classList.add("hidden");
}

function openModalDelete(noteId) {
  pendingDeleteId = noteId;
  modal.classList.remove("hidden");
}
function closeModal() {
  pendingDeleteId = null;
  modal.classList.add("hidden");
}

function normalize(s) {
  return (s || "").toLowerCase().trim();
}

function renderNotes() {
  const q = normalize(searchInput.value);
  const filtered = notes.filter(n => {
    const t = normalize(n.title || n.titulo);
    const c = normalize(n.content || n.contenido);
    return !q || t.includes(q) || c.includes(q);
  });

  notesMeta.textContent = filtered.length
    ? `${filtered.length} nota(s)`
    : "Sin resultados";

  notesList.innerHTML = "";

  if (!filtered.length) {
    const empty = document.createElement("div");
    empty.className = "muted tiny";
    empty.style.padding = "10px";
    empty.textContent = notes.length
      ? "No hay coincidencias. Prueba otra búsqueda."
      : "Aún no tienes notas. Crea la primera con “+ Nueva”.";
    notesList.appendChild(empty);
    return;
  }

  filtered.forEach(n => {
    const id = n.id || n._id;
    const title = n.title || n.titulo || "(Sin título)";
    const content = n.content || n.contenido || "";

    const card = document.createElement("div");
    card.className = "note-card" + (id === selectedId ? " active" : "");
    card.onclick = () => selectNote(id);

    const t = document.createElement("div");
    t.className = "note-title";
    t.textContent = title;

    const p = document.createElement("div");
    p.className = "note-preview";
    p.textContent = content.length > 90 ? content.slice(0, 90) + "…" : content;

    card.appendChild(t);
    card.appendChild(p);
    notesList.appendChild(card);
  });
}

function resetEditor() {
  selectedId = null;
  noteIdEl.value = "";
  noteTitleEl.value = "";
  noteContentEl.value = "";
  deleteBtn.classList.add("hidden");
  lastSaved.textContent = "";
  document.getElementById("editorSub").textContent = "Crea o edita una nota.";
  renderNotes();
}

function selectNote(id) {
  const n = notes.find(x => (x.id || x._id) === id);
  if (!n) return;

  selectedId = id;
  noteIdEl.value = id;
  noteTitleEl.value = n.title || n.titulo || "";
  noteContentEl.value = n.content || n.contenido || "";
  deleteBtn.classList.remove("hidden");
  document.getElementById("editorSub").textContent = `Editando: ${noteTitleEl.value || "nota"}`;
  renderNotes();
}

async function loadNotes() {
  try {
    notesList.innerHTML = `<div class="muted tiny" style="padding:10px;">Cargando notas...</div>`;
    const data = await getNotes();

    // Soporta array directo o { data: [...] }
    const arr = Array.isArray(data) ? data : (data?.data || []);
    notes = arr.map(x => ({
      ...x,
      title: x.title ?? x.titulo,
      content: x.content ?? x.contenido
    }));

    renderNotes();
  } catch (e) {
    if (e.status === 401) {
      showToast("Sesión expirada. Inicia sesión de nuevo.");
      clearToken();
      showAuth();
      return;
    }
    showToast(`No se pudieron cargar notas: ${e.message}`);
    notes = [];
    renderNotes();
  }
}

async function onSave() {
  const title = noteTitleEl.value.trim();
  const content = noteContentEl.value.trim();

  if (!title || !content) {
    showToast("Completa título y contenido.");
    return;
  }

  saveBtn.disabled = true;

  try {
    if (selectedId) {
      // PUT
      await updateNote(selectedId, { title, content });
      showToast("Nota actualizada ✅");
    } else {
      // POST
      const created = await createNote({ title, content });
      showToast("Nota creada ✅");
      // si el backend regresa el id, lo seleccionamos al recargar
      const createdId = created?.id || created?._id || created?.data?.id || created?.data?._id;
      await loadNotes();
      if (createdId) selectNote(createdId);
      saveBtn.disabled = false;
      lastSaved.textContent = `Guardado: ${new Date().toLocaleString()}`;
      return;
    }

    await loadNotes();
    lastSaved.textContent = `Guardado: ${new Date().toLocaleString()}`;
  } catch (e) {
    if (e.status === 401) {
      showToast("Sesión expirada. Inicia sesión de nuevo.");
      clearToken();
      showAuth();
      return;
    }
    showToast(`Error al guardar: ${e.message}`);
  } finally {
    saveBtn.disabled = false;
  }
}

async function onDeleteConfirmed() {
  if (!pendingDeleteId) return;
  try {
    // DELETE
    await deleteNote(pendingDeleteId);
    showToast("Nota eliminada 🗑️");
    closeModal();
    await loadNotes();
    resetEditor();
  } catch (e) {
    closeModal();
    if (e.status === 401) {
      showToast("Sesión expirada. Inicia sesión de nuevo.");
      clearToken();
      showAuth();
      return;
    }
    showToast(`Error al eliminar: ${e.message}`);
  }
}

/* Events */
tabLogin.addEventListener("click", () => setAuthMode("login"));
tabRegister.addEventListener("click", () => setAuthMode("register"));

togglePw.addEventListener("click", () => {
  const isPw = passwordEl.type === "password";
  passwordEl.type = isPw ? "text" : "password";
  togglePw.textContent = isPw ? "Ocultar" : "Ver";
});

authForm.addEventListener("submit", async (ev) => {
  ev.preventDefault();

  const email = emailEl.value.trim();
  const password = passwordEl.value.trim();
  const name = nameEl.value.trim();

  authSubmit.disabled = true;

  try {
    if (mode === "register") {
      await register(email, password, name || undefined);
      showToast("Cuenta creada ✅ Ahora inicia sesión.");
      setAuthMode("login");
      authSubmit.disabled = false;
      return;
    }

    const res = await login(email, password);
    const token = res?.token || res?.data?.token;
    if (!token) throw new Error("No se recibió token del backend.");

    setToken(token);
    showApp(email);
    resetEditor();
    await loadNotes();
    showToast("Sesión iniciada ✅");
  } catch (e) {
    showToast(`Auth: ${e.message}`);
  } finally {
    authSubmit.disabled = false;
  }
});

logoutBtn.addEventListener("click", () => {
  clearToken();
  showToast("Sesión cerrada.");
  showAuth();
});

searchInput.addEventListener("input", renderNotes);
newNoteBtn.addEventListener("click", () => {
  resetEditor();
  noteTitleEl.focus();
  showToast("Escribe título y contenido y presiona Guardar");
});

clearBtn.addEventListener("click", resetEditor);
saveBtn.addEventListener("click", onSave);

deleteBtn.addEventListener("click", () => {
  const id = selectedId;
  if (!id) return;
  openModalDelete(id);
});

modalCancel.addEventListener("click", closeModal);
modalConfirm.addEventListener("click", onDeleteConfirmed);

// Init
(function init() {
  setAuthMode("login");
  const token = getToken();
  if (token) {
    showApp("");
    loadNotes();
  } else {
    showAuth();
  }
})();
