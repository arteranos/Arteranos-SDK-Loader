# Arteranos SDK loader

This .unitypackage file causes the Unity Editor to load the Arteranos SDK into your project. Just import this file with the **Asset** -> **Import Package** menu item.

Once done, two menu headers - "Tools" and "Arteranos" - will appear and the imported loader deletes itself, to avoid clutter.

## How it works

The SDK consists of four packages (this time, the ones residing in Unity's package manager window, just to clarify...), which will been added into the project's manifest file, which will be reloaded.

## Why?

1. Three of four of the packages have a shared use - be it in the Arteranos client itself, and in the SDK for the world template creation. And one of them has some potential use, outside of Arteranos. So, to avoid code duplication, they reside in separate repositories.
   
2. Deploying the packages into a Unity Package Registry host is too much hassle, maintaining the packages in yet another cloud host. The packages have a highly specific use - for Arteranos itself and for its world building - and yet some public interest, especially for the latter use case.
   
3. Manually installing four packages, one by one, is error prone, especially with their dependencies.