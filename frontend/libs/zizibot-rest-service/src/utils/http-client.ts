import axios from 'axios';
import Cookies from 'js-cookie';


const apiClient = axios.create({
  baseURL: process.env.API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer ' + Cookies.get('bearerToken')
  }
});

export default apiClient;