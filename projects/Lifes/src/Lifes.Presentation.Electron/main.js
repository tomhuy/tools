const { app, BrowserWindow } = require('electron');
const path = require('path');
const fs = require('fs');
const { spawn } = require('child_process');
const log = require('electron-log');

const isDev = process.env.NODE_ENV === 'development';

// Cấu hình log - File log sẽ được lưu tại thư mục chứa file chạy (exe)
const logFolder = isDev ? path.join(__dirname, 'logs') : path.join(path.dirname(process.execPath), 'logs');
const logFilePath = path.join(logFolder, 'main.log');

// Đảm bảo thư mục log tồn tại để tránh crash
if (!fs.existsSync(logFolder)) {
  fs.mkdirSync(logFolder, { recursive: true });
}

log.transports.file.resolvePath = () => logFilePath;
log.transports.file.level = 'info';

log.info('App starting...');
log.info('Log file path:', logFilePath);

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

function startDotNetApi() {
  const isDev = process.env.NODE_ENV === 'development';
  if (isDev) {
    log.info("Running in Dev mode. Please ensure WebAPI is running on https://localhost:7119 or http://localhost:5242 via Visual Studio / dotnet run");
  } else {
    const apiPath = path.join(process.resourcesPath, 'backend', 'Lifes.Presentation.WebApi.exe');
    
    // Khởi tạo biến môi trường mới bằng cách kế thừa môi trường hiện tại
    const backendEnv = Object.assign({}, process.env, {
        ASPNETCORE_ENVIRONMENT: 'Production', // Báo cho .NET biết đây là bản build Release
        ASPNETCORE_URLS: 'http://localhost:5110', // Truyền port mặc định (hoặc logic lấy port động nếu cần)
    });
    
    log.info("Starting backend at:", apiPath);
    
    apiProcess = spawn(apiPath, [], {
        env: backendEnv,       // Truyền Environment Variables vào đây
        windowsHide: true,     // Giấu cửa sổ terminal đen thui
        stdio: 'pipe'          // Capture stdout/stderr
    });

    apiProcess.stdout.on('data', (data) => {
        log.info(`[Backend API] ${data}`);
    });

    apiProcess.stderr.on('data', (data) => {
        log.error(`[Backend API Error] ${data}`);
    });

    apiProcess.on('close', (code) => {
        log.info(`Backend process exited with code ${code}`);
    });
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
