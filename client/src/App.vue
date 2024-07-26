<template>
  <v-app>
    <MainNavbar />

    <v-main>
      <router-view></router-view>
    </v-main>

    <v-footer app>
      <span>&copy; {{ new Date().getFullYear() }} Мій Магазин</span>
    </v-footer>
  </v-app>
</template>

<script>
  import MainNavbar from '@/components/shared/MainNavbar.vue'

  export default {
    name: 'App',
    components: {
      MainNavbar
    },
    
  }
</script>