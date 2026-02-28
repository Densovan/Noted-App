import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import api from '../api/axios';

interface User {
  username: string;
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(JSON.parse(localStorage.getItem('user') || 'null'));
  const token = ref<string | null>(localStorage.getItem('token'));

  const isAuthenticated = computed(() => !!token.value);

  async function login(credentials: any) {
    const response = await api.post('/auth/login', credentials);
    token.value = response.data.token;
    user.value = { username: response.data.username };
    localStorage.setItem('token', token.value!);
    localStorage.setItem('user', JSON.stringify(user.value));
  }

  async function register(userData: any) {
    await api.post('/auth/register', userData);
  }

  function logout() {
    token.value = null;
    user.value = null;
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  return { user, token, isAuthenticated, login, register, logout };
});
