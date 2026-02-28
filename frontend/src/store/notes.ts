import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '../api/axios';

export interface Note {
  id: number;
  title: string;
  content: string | null;
  createdAt: string;
  updatedAt: string;
}

export const useNotesStore = defineStore('notes', () => {
  const notes = ref<Note[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  async function fetchNotes() {
    loading.value = true;
    error.value = null;
    try {
      const response = await api.get('/notes');
      notes.value = response.data;
    } catch (err: any) {
      error.value = 'Failed to fetch notes';
      console.error(err);
    } finally {
      loading.value = false;
    }
  }

  async function addNote(note: { title: string; content: string | null }) {
    try {
      const response = await api.post('/notes', note);
      notes.value.unshift(response.data);
    } catch (err) {
      console.error(err);
      throw err;
    }
  }

  async function updateNote(id: number, note: { title: string; content: string | null }) {
    try {
      const response = await api.put(`/notes/${id}`, note);
      const index = notes.value.findIndex((n) => n.id === id);
      if (index !== -1) {
        notes.value[index] = response.data;
      }
    } catch (err) {
      console.error(err);
      throw err;
    }
  }

  async function deleteNote(id: number) {
    try {
      await api.delete(`/notes/${id}`);
      notes.value = notes.value.filter((n) => n.id !== id);
    } catch (err) {
      console.error(err);
      throw err;
    }
  }

  return { notes, loading, error, fetchNotes, addNote, updateNote, deleteNote };
});
