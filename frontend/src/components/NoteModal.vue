<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-950/40 backdrop-blur-md animate-fade-in">
    <div class="w-full max-w-3xl glass rounded-[2.5rem] border border-white/10 shadow-[0_25px_50px_-12px_rgba(0,0,0,0.5)] overflow-hidden animate-slide-up">
      <!-- Modal Header -->
      <div class="p-8 border-b border-white/5 flex justify-between items-center bg-white/5">
        <div class="flex items-center gap-3">
          <div class="p-2.5 rounded-xl bg-primary-600/20 text-primary-400">
            <PencilLineIcon v-if="note" class="w-6 h-6" />
            <PlusIcon v-else class="w-6 h-6" />
          </div>
          <div>
            <h2 class="text-2xl font-black text-white tracking-tight">{{ note ? 'Edit Note' : 'Create New Note' }}</h2>
            <p class="text-xs font-bold text-slate-500 uppercase tracking-widest mt-0.5">{{ note ? 'Update your thoughts' : 'Begin a new idea' }}</p>
          </div>
        </div>
        <button @click="$emit('close')" class="p-2 text-slate-500 hover:text-white hover:bg-white/5 rounded-xl transition-all active:scale-90">
          <XIcon class="w-6 h-6" />
        </button>
      </div>

      <!-- Modal Body -->
      <div class="p-8 space-y-8 bg-slate-900/20">
        <div class="space-y-2">
          <label class="block text-xs font-bold uppercase tracking-widest text-slate-500 ml-1">The Headline</label>
          <input v-model="form.title" type="text" placeholder="Something brilliant..."
            class="w-full px-6 py-5 bg-slate-950/50 border border-slate-800 rounded-2xl focus:ring-2 focus:ring-primary-500/50 outline-none text-white text-2xl font-black placeholder-slate-700 transition-all border-none" />
        </div>
        
        <div class="space-y-2">
          <label class="block text-xs font-bold uppercase tracking-widest text-slate-500 ml-1">The Details</label>
          <textarea v-model="form.content" rows="12" placeholder="Start your creative flow here..."
            class="w-full px-6 py-5 bg-slate-950/50 border border-slate-800 rounded-2xl focus:ring-2 focus:ring-primary-500/50 outline-none text-white text-lg font-medium resize-none placeholder-slate-700 transition-all border-none"></textarea>
        </div>
      </div>

      <!-- Modal Footer -->
      <div class="p-8 px-10 bg-white/5 border-t border-white/5 flex flex-col sm:flex-row justify-between items-center gap-4">
        <div class="flex flex-col text-[10px] font-bold text-slate-500 uppercase tracking-widest">
          <div v-if="note" class="flex items-center gap-1.5">
            <div class="w-1 h-1 rounded-full bg-slate-700"></div>
            Last sync: {{ formatDateTime(note.updatedAt) }}
          </div>
        </div>
        
        <div class="flex gap-4 w-full sm:w-auto">
          <button @click="$emit('close')" 
            class="flex-1 sm:flex-none px-8 py-4 text-slate-400 hover:text-white font-bold transition-all">
            Dismiss
          </button>
          <button @click="save" :disabled="!form.title"
            class="flex-1 sm:flex-none px-10 py-4 bg-primary-600 hover:bg-primary-500 disabled:opacity-50 text-white font-black rounded-2xl shadow-[0_10px_25px_rgba(99,102,241,0.3)] hover:shadow-[0_15px_30px_rgba(99,102,241,0.4)] transition-all duration-300 active:scale-95 flex items-center justify-center gap-2">
            <span>Commit Note</span>
            <CheckIcon class="w-5 h-5" />
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, onMounted } from 'vue';
import { useNotesStore } from '../store/notes';
import type { Note } from '../store/notes';
import { XIcon, PencilLineIcon, PlusIcon, CheckIcon } from 'lucide-vue-next';

const props = defineProps<{
  note: Note | null;
}>();

const emit = defineEmits(['close', 'saved']);
const notesStore = useNotesStore();

const form = reactive({
  title: '',
  content: '',
});

onMounted(() => {
  if (props.note) {
    form.title = props.note.title;
    form.content = props.note.content || '';
  }
});

const save = async () => {
  if (!form.title) return;
  
  try {
    if (props.note) {
      await notesStore.updateNote(props.note.id, form);
    } else {
      await notesStore.addNote(form);
    }
    emit('saved');
  } catch (err) {
    alert('Failed to save note. Please try again.');
  }
};

const formatDateTime = (dateStr: string) => {
  return new Date(dateStr).toLocaleString(undefined, {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};
</script>
