import { createRouter, createWebHistory } from 'vue-router'
import ProductDetail from '@/components/ProductDetail.vue'
import AddProduct from '@/components/AddProduct.vue'
import Main from '@/components/Main.vue'

const routes = [
  {
    path: '/product/:id',
    name: 'ProductDetail',
    component: ProductDetail,
    props: true
  },
  {
    path: '/add-product',
    name: 'AddProduct',
    component: AddProduct
  },
  {
    path: '/',
    name: 'MainComponent',
    component: Main
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router