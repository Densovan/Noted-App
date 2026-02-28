<template>
  <div class="relative min-h-screen flex items-center justify-center p-4 overflow-hidden bg-slate-950">
    <!-- Mesh Gradient Background -->
    <div class="absolute inset-0 -z-10">
      <div class="absolute top-[-10%] left-[-10%] w-[40%] h-[40%] rounded-full bg-primary-900/40 blur-[120px] animate-pulse"></div>
      <div class="absolute bottom-[-10%] right-[-10%] w-[40%] h-[40%] rounded-full bg-indigo-900/30 blur-[120px] animate-pulse" style="animation-delay: 1s;"></div>
    </div>

    <!-- Login/Register Card -->
    <div class="w-full max-w-md p-8 glass rounded-[2rem] shadow-2xl border border-white/5 animate-fade-in">
      <div class="text-center mb-10">
        <div class="inline-flex items-center justify-center w-16 h-16 rounded-2xl bg-primary-600/20 text-primary-400 mb-6 group">
          <BookMarkedIcon class="w-8 h-8 group-hover:scale-110 transition-transform duration-300" />
        </div>
        <h1 class="text-4xl font-extrabold tracking-tight text-white mb-2">{{ isLogin ? 'Welcome Back' : 'Join Notes' }}</h1>
        <p class="text-slate-400 text-sm font-medium">{{ isLogin ? 'Sign in to your premium workspace' : 'Create an account to get started' }}</p>
      </div>

      <form @submit.prevent="handleSubmit" class="space-y-6">
        <div class="space-y-2">
          <label class="block text-xs font-bold uppercase tracking-widest text-slate-500 ml-1">Username</label>
          <div class="relative group">
            <UserIcon class="absolute left-3.5 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-500 group-focus-within:text-primary-400 transition-colors" />
            <input v-model="form.username" type="text" required placeholder="yourname"
              class="w-full pl-11 pr-4 py-3.5 bg-slate-900/50 border border-slate-800 rounded-2xl focus:ring-2 focus:ring-primary-500 focus:border-transparent outline-none transition-all text-white placeholder-slate-600" />
          </div>
        </div>

        <div class="space-y-2">
          <label class="block text-xs font-bold uppercase tracking-widest text-slate-500 ml-1">Password</label>
          <div class="relative group">
            <LockIcon class="absolute left-3.5 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-500 group-focus-within:text-primary-400 transition-colors" />
            <input v-model="form.password" type="password" required placeholder="••••••••"
              class="w-full pl-11 pr-4 py-3.5 bg-slate-900/50 border border-slate-800 rounded-2xl focus:ring-2 focus:ring-primary-500 focus:border-transparent outline-none transition-all text-white placeholder-slate-600" />
          </div>
        </div>

        <button type="submit" :disabled="loading"
          class="group relative w-full py-4 px-6 bg-primary-600 hover:bg-primary-500 disabled:opacity-50 text-white font-bold rounded-2xl shadow-[0_0_20px_rgba(99,102,241,0.3)] hover:shadow-[0_0_25px_rgba(99,102,241,0.5)] transition-all duration-300 active:scale-[0.98]">
          <span class="flex items-center justify-center gap-2">
            <Loader2Icon v-if="loading" class="w-5 h-5 animate-spin" />
            {{ isLogin ? 'Sign In' : 'Create Account' }}
            <ArrowRightIcon v-if="!loading" class="w-5 h-5 group-hover:translate-x-1 transition-transform" />
          </span>
          <div class="absolute inset-x-0 bottom-0 h-px bg-gradient-to-r from-transparent via-white/20 to-transparent"></div>
        </button>
      </form>

      <div class="mt-10 text-center">
        <p class="text-sm text-slate-500">
          {{ isLogin ? "New to Notes?" : "Already have an account?" }}
          <button @click="toggleMode" class="ml-1 text-primary-400 hover:text-primary-300 font-bold underline-offset-4 hover:underline transition-all">
            {{ isLogin ? 'Register now' : 'Log in here' }}
          </button>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '../store/auth';
import { BookMarkedIcon, UserIcon, LockIcon, ArrowRightIcon, Loader2Icon } from 'lucide-vue-next';

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();

const isLogin = computed(() => route.path === '/login');
const loading = ref(false);
const form = reactive({
  username: '',
  password: '',
});

const toggleMode = () => {
  router.push(isLogin.value ? '/register' : '/login');
};

const handleSubmit = async () => {
  loading.value = true;
  try {
    if (isLogin.value) {
      await authStore.login(form);
      router.push('/');
    } else {
      await authStore.register(form);
      alert('Registration successful! Please login.');
      router.push('/login');
    }
  } catch (err) {
    alert('Authentication failed. Please check your credentials.');
  } finally {
    loading.value = false;
  }
};
</script>
