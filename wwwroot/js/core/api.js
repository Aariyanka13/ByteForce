/**
 * Universal Fetch API Wrapper for Backend Integration
 */
const Api = {
  async request(endpoint, options = {}) {
    const token = Storage.getToken();
    const headers = options.headers || {};

    if (token && !headers['Authorization']) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    if (options.body && !(options.body instanceof FormData) && !headers['Content-Type']) {
      headers['Content-Type'] = 'application/json';
      options.body = JSON.stringify(options.body);
    }

    const config = {
      method: options.method || 'GET',
      headers,
      ...options
    };

    try {
      const response = await fetch(endpoint, config);

      if (response.status === 401) {
        // Unauthenticated -> clear session and redirect to login if protected page
        Storage.clearSession();
        if (!window.location.pathname.includes('/auth/')) {
          window.location.href = CONSTANTS.ROUTES.PUBLIC.LOGIN;
        }
        throw new Error('Unauthorized session. Please log in again.');
      }

      if (response.status === 204) {
        return null;
      }

      // Read response content as text first
      const text = await response.text();
      let data = null;
      if (text && text.trim().length > 0) {
        try {
          data = JSON.parse(text);
        } catch {
          data = text;
        }
      }

      if (!response.ok) {
        let message = 'An unexpected error occurred.';
        if (data && typeof data === 'object') {
          if (data.errors && typeof data.errors === 'object') {
            const errList = [];
            Object.values(data.errors).forEach(err => {
              if (Array.isArray(err)) {
                errList.push(...err);
              } else if (typeof err === 'string') {
                errList.push(err);
              }
            });
            if (errList.length > 0) {
              message = errList.join(' ');
            } else {
              message = data.message || data.title || data.error || JSON.stringify(data);
            }
          } else {
            message = data.message || (data.title && data.title !== 'One or more validation errors occurred.' ? data.title : null) || data.error || JSON.stringify(data);
          }
        } else if (typeof data === 'string') {
          message = data;
        }
        const error = new Error(message);
        error.status = response.status;
        error.data = data;
        throw error;
      }

      return data;
    } catch (err) {
      console.error(`API [${config.method} ${endpoint}] Error:`, err);
      throw err;
    }
  },

  get(endpoint, queryParams = null) {
    let url = endpoint;
    if (queryParams) {
      const cleanParams = new URLSearchParams();
      Object.keys(queryParams).forEach(key => {
        const val = queryParams[key];
        if (val !== null && val !== undefined && val !== '') {
          cleanParams.append(key, val);
        }
      });
      const queryString = cleanParams.toString();
      if (queryString) {
        url += (url.includes('?') ? '&' : '?') + queryString;
      }
    }
    return this.request(url, { method: 'GET' });
  },

  post(endpoint, body = {}) {
    return this.request(endpoint, { method: 'POST', body });
  },

  put(endpoint, body = {}) {
    return this.request(endpoint, { method: 'PUT', body });
  },

  patch(endpoint, body = {}) {
    return this.request(endpoint, { method: 'PATCH', body });
  },

  delete(endpoint) {
    return this.request(endpoint, { method: 'DELETE' });
  },

  upload(endpoint, file) {
    const formData = new FormData();
    formData.append('file', file);
    return this.request(endpoint, {
      method: 'POST',
      body: formData
    });
  },

  async download(endpoint, fallbackFileName = 'download.pdf') {
    const token = Storage.getToken();
    const headers = {};
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(endpoint, { method: 'GET', headers });
    if (!response.ok) {
      throw new Error(`Failed to download file (Status ${response.status})`);
    }

    const blob = await response.blob();
    const disposition = response.headers.get('content-disposition');
    let fileName = fallbackFileName;
    if (disposition && disposition.includes('filename=')) {
      const match = disposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
      if (match && match[1]) {
        fileName = match[1].replace(/['"]/g, '');
      }
    }

    const downloadUrl = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = downloadUrl;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(downloadUrl);
  }
};
