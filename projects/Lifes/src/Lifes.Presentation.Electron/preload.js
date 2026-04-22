const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('electronAPI', {
  // Mở rộng các hàm giao tiếp Electron Main qua IPCRenderer
  // Ví dụ:
  // minimize: () => ipcRenderer.send('window-minimize'),
  // close: () => ipcRenderer.send('window-close'),
});
