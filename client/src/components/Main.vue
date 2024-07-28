<template>
    <v-container fluid>
      <v-carousel show-arrows="hover" cycle hide-delimiter-background>
        <v-carousel-item
          src="https://images.unsplash.com/photo-1721213126988-3450888e603a?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"
          cover
        ></v-carousel-item>
        <v-carousel-item
          src="https://images.unsplash.com/photo-1721148774311-5634b1de8cfc?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"
          cover
        ></v-carousel-item>
  
        <v-carousel-item
          src="https://images.unsplash.com/photo-1721213375079-1a626470ecfe?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"
          cover
        ></v-carousel-item>
      </v-carousel>
    </v-container>
  </template>
  
  <script>
  import axios from "axios";
  
  export default {
    name: "MainComponent",
    components: {},
    data() {
      return {
        categories: [],
      };
    },
    mounted() {
      this.fetchCategories();
    },
    methods: {
      async fetchCategories() {
        try {
          const response = await axios.get("https://localhost:44329/api/Category/getAll");
          this.categories = response.data;
          console.log(this.categories); // Перевірка отриманих даних
        } catch (error) {
          console.error("Error fetching categories:", error);
        }
      },
    },
  };
  </script>
  
  <style></style>
  