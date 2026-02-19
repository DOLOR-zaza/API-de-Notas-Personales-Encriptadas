const API_BASE_URL = "http://localhost:5063"; 

function getToken() {
  return localStorage.getItem("token");
}
function setToken(token) {
  localStorage.setItem("token", token);
}
function clearToken() {
  localStorage.removeItem("token");
}

async function apiFetch(path, { method = "GET", body = null, auth = true } = {}) {
  const headers = { "Content-Type": "application/json" };
  if (auth) {
    const token = getToken();
    if (token) headers["Authorization"] = `Bearer ${token}`;
  }

  const res = await fetch(`${API_BASE_URL}${path}`, {
    method,
    headers,
    body: body ? JSON.stringify(body) : null,
  });

  // Lee JSON o texto según lo que venga
  let data = null;
  const text = await res.text();
  try { data = text ? JSON.parse(text) : null; } catch { data = text || null; }

  if (!res.ok) {
    const message = (data && (data.message || data.error)) || `Error ${res.status}`;
    const err = new Error(message);
    err.status = res.status;
    err.data = data;
    throw err;
  }

  return data;
}

/* AUTH */
async function login(email, password) {
  return apiFetch("/api/auth/login", { method: "POST", body: { email, password }, auth: false });
}
async function register(email, password, name) {
  // ajusta el payload según lo que el backend espere (ej: { name, email, password })
  const payload = name ? { email, password, name } : { email, password };
  return apiFetch("/api/auth/register", { method: "POST", body: payload, auth: false });
}

/* NOTES CRUD */
async function getNotes() {
  return apiFetch("/api/notes");
}
async function createNote(note) {
  return apiFetch("/api/notes", { method: "POST", body: note });
}
async function updateNote(id, note) {
  return apiFetch(`/api/notes/${id}`, { method: "PUT", body: note });
}
async function deleteNote(id) {
  return apiFetch(`/api/notes/${id}`, { method: "DELETE" });
}
