const { app, BrowserWindow } = require('electron');
const path = require('path');
const { spawn } = require('child_process');

let mainWindow;
let apiProcess;

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 1280,
    height: 800,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      nodeIntegration: false,
    },
  });

  const isDev = process.env.NODE_ENV === 'development';

  if (isDev) {
    mainWindow.loadURL('http://localhost:4200');
    mainWindow.webContents.openDevTools();
  } else {
    mainWindow.loadFile(path.join(__dirname, 'dist', 'frontend', 'browser', 'index.html'));
  }

  mainWindow.on('closed', function () {
    mainWindow = null;
  });
}

// In a real application, you would spawn the C# WebAPI executable here
function startDotNetApi() {
  const isDev = process.env.NODE_ENV === 'development';
  if (isDev) {
    console.log("Running in Dev mode. Please ensure WebAPI is running on https://localhost:7119 or http://localhost:5242 via Visual Studio / dotnet run");
  } else {
    // const apiPath = path.join(process.resourcesPath, 'api', 'Lifes.Presentation.WebApi.exe');
    // apiProcess = spawn(apiPath);
  }
}

app.on('ready', () => {
  startDotNetApi();
  createWindow();
});

app.on('window-all-closed', function () {
  if (process.platform !== 'darwin') {
    if (apiProcess) apiProcess.kill();
    app.quit();
  }
});

app.on('activate', function () {
  if (mainWindow === null) {
    createWindow();
  }
});
