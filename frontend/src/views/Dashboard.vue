<template>
  <div class="min-h-screen bg-slate-950 font-sans selection:bg-primary-500/30">
    <!-- Main Content Area -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 animate-fade-in">
      
      <!-- Premium Header -->
      <header class="flex flex-col md:flex-row md:items-end justify-between gap-8 mb-16 px-2">
        <div class="space-y-2">
          <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-primary-500/10 border border-primary-500/20 text-primary-400 text-xs font-bold uppercase tracking-widest mb-2">
            <SparklesIcon class="w-3.5 h-3.5" />
            Workspace
          </div>
          <h1 class="text-5xl font-black text-white tracking-tight">
            My <span class="text-transparent bg-clip-text bg-gradient-to-r from-primary-400 to-indigo-400">Notes</span>
          </h1>
          <p class="text-slate-500 text-lg font-medium max-w-md">Organize your creative thoughts and projects in one elegant space.</p>
        </div>
        
        <div class="flex items-center gap-4">
          <button @click="openModal()"
            class="group flex items-center gap-2 px-8 py-4 bg-primary-600 hover:bg-primary-500 text-white font-bold rounded-2xl shadow-[0_10px_30px_rgba(99,102,241,0.3)] transition-all duration-300 active:scale-95">
            <PlusIcon class="w-5 h-5 group-hover:rotate-90 transition-transform duration-300" />
            Create Note
          </button>
          
          <button @click="logout"
            class="p-4 text-slate-400 hover:text-white glass-card rounded-2xl transition-all duration-300 hover:border-white/20 active:scale-90"
            title="Logout">
            <LogOutIcon class="w-6 h-6" />
          </button>
        </div>
      </header>

      <!-- Advanced Toolbar -->
      <div class="flex flex-col md:flex-row gap-6 mb-12 items-center px-2">
        <div class="relative w-full md:flex-1 group">
          <SearchIcon class="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-500 group-focus-within:text-primary-400 transition-colors" />
          <input v-model="searchQuery" type="text" placeholder="Search across your notes..."
            class="w-full pl-12 pr-6 py-4 bg-slate-900/40 border border-slate-800 rounded-2xl focus:ring-2 focus:ring-primary-500/50 focus:border-primary-500 outline-none text-white transition-all placeholder-slate-600" />
        </div>
        
        <div class="flex gap-4 w-full md:w-auto">
          <div class="relative w-full md:w-48">
            <select v-model="sortBy"
              class="w-full appearance-none px-6 py-4 bg-slate-900/40 border border-slate-800 rounded-2xl focus:ring-2 focus:ring-primary-500/50 outline-none text-white transition-all cursor-pointer">
              <option value="newest">Latest First</option>
              <option value="oldest">Oldest First</option>
              <option value="title">Alphabetical</option>
            </select>
            <ChevronDownIcon class="absolute right-4 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-500 pointer-events-none" />
          </div>
        </div>
      </div>

      <!-- Notes Grid -->
      <div v-if="loading" class="flex flex-col items-center justify-center py-32 space-y-4">
        <div class="relative w-16 h-16">
          <div class="absolute inset-0 rounded-full border-4 border-primary-500/20 border-t-primary-500 animate-spin"></div>
        </div>
        <p class="text-slate-500 font-bold animate-pulse">Syncing your space...</p>
      </div>

      <div v-else-if="filteredNotes.length === 0" class="flex flex-col items-center justify-center py-32 glass rounded-[2.5rem] border-dashed border-2 border-slate-800 animate-slide-up">
        <div class="w-20 h-20 rounded-3xl bg-slate-900 flex items-center justify-center mb-6 text-slate-700">
          <FileTextIcon class="w-10 h-10" />
        </div>
        <h3 class="text-2xl font-bold text-slate-300 mb-2">No notes found</h3>
        <p class="text-slate-500 mb-8 max-w-xs text-center">Your notes library is empty. Let's capture your first brilliant idea today.</p>
        <button @click="openModal()" class="text-primary-400 hover:text-primary-300 font-bold flex items-center gap-2 transition-all">
          <PlusIcon class="w-5 h-5" />
          Start Writing
        </button>
      </div>

      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8 px-2">
        <div v-for="(note, index) in filteredNotes" :key="note.id" 
          @click="openModal(note)"
          class="group relative h-72 p-8 bg-slate-900/50 hover:bg-slate-900 border border-slate-800/60 hover:border-primary-500/30 rounded-[2rem] transition-all duration-500 cursor-pointer shadow-sm hover:shadow-[0_20px_40px_rgba(0,0,0,0.4)] hover:-translate-y-2 overflow-hidden animate-slide-up"
          :style="{ animationDelay: `${index * 50}ms` }">
          
          <!-- Decorative element -->
          <div class="absolute top-0 right-0 w-32 h-32 bg-primary-600/5 blur-3xl opacity-0 group-hover:opacity-100 transition-opacity"></div>
          
          <div class="relative h-full flex flex-col">
            <div class="flex justify-between items-start mb-4">
              <h3 class="text-2xl font-bold text-white group-hover:text-primary-400 transition-colors line-clamp-2 leading-tight">{{ note.title }}</h3>
              <div class="p-2 opacity-0 group-hover:opacity-100 transition-all transform translate-x-2 group-hover:translate-x-0">
                <ArrowUpRightIcon class="w-5 h-5 text-primary-400" />
              </div>
            </div>
            
            <p class="text-slate-400 line-clamp-4 mb-6 text-sm leading-relaxed">{{ note.content || 'Your note has no content yet...' }}</p>
            
            <div class="mt-auto pt-6 border-t border-slate-800/50 flex items-center justify-between">
              <div class="flex items-center gap-2 text-slate-500">
                <CalendarIcon class="w-3.5 h-3.5" />
                <span class="text-xs font-bold uppercase tracking-wider">{{ formatDate(note.createdAt) }}</span>
              </div>
              <button @click.stop="confirmDelete(note.id)" 
                class="p-2 text-slate-600 hover:text-red-400 hover:bg-red-400/10 rounded-xl transition-all active:scale-90">
                <Trash2Icon class="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Note Modal -->
    <NoteModal v-if="showModal" :note="selectedNote" @close="closeModal" @saved="onSaved" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useNotesStore } from '../store/notes';
import type { Note } from '../store/notes';
import { useAuthStore } from '../store/auth';
import { 
  PlusIcon, 
  LogOutIcon, 
  SearchIcon, 
  Trash2Icon, 
  SparklesIcon, 
  ChevronDownIcon,
  FileTextIcon,
  CalendarIcon,
  ArrowUpRightIcon
} from 'lucide-vue-next';
import NoteModal from '../components/NoteModal.vue';

const router = useRouter();
const notesStore = useNotesStore();
const authStore = useAuthStore();

const loading = computed(() => notesStore.loading);
const searchQuery = ref('');
const sortBy = ref('newest');
const showModal = ref(false);
const selectedNote = ref<Note | null>(null);

const filteredNotes = computed(() => {
  let result = [...notesStore.notes];

  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase();
    result = result.filter(n => n.title.toLowerCase().includes(q) || n.content?.toLowerCase().includes(q));
  }

  result.sort((a, b) => {
    if (sortBy.value === 'newest') return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
    if (sortBy.value === 'oldest') return new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();
    if (sortBy.value === 'title') return a.title.localeCompare(b.title);
    return 0;
  });

  return result;
});

const openModal = (note: Note | null = null) => {
  selectedNote.value = note;
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  selectedNote.value = null;
};

const onSaved = () => {
  closeModal();
};

const confirmDelete = async (id: number) => {
  if (confirm('Are you sure you want to permanentely remove this note?')) {
    await notesStore.deleteNote(id);
  }
};

const logout = () => {
  authStore.logout();
  router.push('/login');
};

const formatDate = (dateStr: string) => {
  return new Date(dateStr).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  });
};

onMounted(() => {
  notesStore.fetchNotes();
});
</script>
