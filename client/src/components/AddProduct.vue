<template>
    <v-container>
      <v-form @submit.prevent="submitForm">
        <v-text-field v-model="product.name" label="Product Name"></v-text-field>
        <v-text-field v-model="product.price" label="Price"></v-text-field>
        <v-textarea v-model="product.description" label="Description"></v-textarea>
        <v-text-field v-model="product.imageUrl" label="Image URL"></v-text-field>
        <v-btn type="submit" color="primary">Add Product</v-btn>
      </v-form>
    </v-container>
  </template>
  
  <script>
  import api from '../api';
  
  export default {
    data() {
      return {
        product: {
          name: '',
          price: '',
          description: '',
          imageUrl: ''
        }
      };
    },
    methods: {
      submitForm() {
        api.addProduct(this.product).then(() => {
          this.$router.push('/');
        });
      }
    }
  };
  </script>
  