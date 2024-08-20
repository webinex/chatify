import axios from 'axios';

function create(baseURL: string) {
  const instance = axios.create({ baseURL });
  instance.interceptors.request.use((request) => {
    request.headers['X-USER-ID'] = localStorage.getItem('chatify://me');
    return request;
  });

  return instance;
}

export const CHATIFY_AXIOS = create('/api/chatify');
export const FLIPPO_AXIOS = create('/api/flippo');
