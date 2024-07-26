<template>
  <v-container>
    <v-row>
      <v-col>
        <v-card>
          <v-img :src="product.imageUrl" height="200px"></v-img>
          <v-card-title>{{ product.id }}</v-card-title>
          <v-card-subtitle>{{ product.firstName }}</v-card-subtitle>
          <v-card-text>{{ product.description }}</v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
  import api from '../api';

  export default {
    props: ['id'],
    data() {
      return {
        product: {
          id: Number,
          name: String
        }
      };
    },
    created() {
      api.getProduct(this.id).then(response => {
        this.product = response.data;
      });
    }
  };
</script>