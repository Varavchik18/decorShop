import axios from 'axios';

class ApiService {
  constructor() {
    this.client = axios.create({
      baseURL: process.env.VUE_APP_API_URL || 'https://localhost:8081/api',
      withCredentials: false,
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json'
      }
    });

    this.client.interceptors.request.use(this.handleRequestSuccess, this.handleRequestError);
    this.client.interceptors.response.use(this.handleResponseSuccess, this.handleResponseError);
  }

  handleRequestSuccess(config) {
    // Можна додати логіку для додавання токена аутентифікації тощо
    return config;
  }

  handleRequestError(error) {
    return Promise.reject(error);
  }

  handleResponseSuccess(response) {
    return response;
  }

  handleResponseError(error) {
    // Обробка помилок відповіді (наприклад, оновлення токена)
    return Promise.reject(error);
  }

  async request({ method, url, data = null, params = null }) {
    try {
      const response = await this.client.request({
        method,
        url,
        data,
        params
      });
      return response.data;
    } catch (error) {
      this.handleError(error);
      throw error;
    }
  }

  handleError(error) {
    // Централізована обробка помилок
    console.error('API Error:', error);
    // Можна додати логіку для сповіщення користувача про помилку
  }

  get(url, params) {
    return this.request({ method: 'get', url, params });
  }

  post(url, data) {
    return this.request({ method: 'post', url, data });
  }

  put(url, data) {
    return this.request({ method: 'put', url, data });
  }

  delete(url) {
    return this.request({ method: 'delete', url });
  }
}

export const apiService = new ApiService();