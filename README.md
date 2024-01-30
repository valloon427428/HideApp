# HideApp

C# background app that can hide other process/window on screenshot and taskbar.

## How to use

`HideAppHelper` class provides all hide-app features including hide/unhude on screenshot, hide on taskbar, remove title, recover window and so on.

`HotKeyManager` class provides Global HotKey settings.

This app uses third-party library to inject hide/unhide API: 
https://github.com/radiantly/Invisiwind

HotKeys:
- `Alt + Z` : Hide on screenshot, hide on taskbar and remove window title for current active window. (You have to set focus on the target window before click this hotkey, or other window will be affected.)
- `Alt + Win + Ctrl + Z` : Unhide on screenshot. (This works wrong on some apps, so I recommend restarting the app to unhide or reset.)
- `Alt + Win + Ctrl + X` : Recover hidden & minized window. (When you hide an app and minimize it, you can't recover it from taskbar or Alt + TAB. You can recover the last one by this hotkey. If you hide 2 or more apps, or you restart this app, you can't recover those windows. You can forcibly close and restart.)


## PS
This app has no interface, no taskbar icon and no tray icon. You can shut down this app on Task Manager.

## Important
This feature works properly on Microsoft Windows 10 version 2004 and above.