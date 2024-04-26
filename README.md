# FoodMenuAR
## DESCRIPTION
This is a basic implementation of an AR menu.  
This project isn't for production, it covers the goals that I conceived and are described below. Therefore, some parts of this project may seem strange to you. You are free to use it for any purposes and adapt it according to your needs.  

## MAIN GOALS
Initially, this project was designed to help me pass a university subject. However, after a few years, I decided to redesign it to use as a portfolio.  
I aimed to focus on:
- System that would be easy to extend with new data and menus;  
- Logic only by using standard Unity libraries;


I also wanted to work with several things:
- New Unity 6 version;  
- Augmented Reality;  
- New UI Toolkit;  
- Addressables;  

## GENERAL IMPLEMENTED FEATURES
### SHOWCASE
![](https://github.com/Bdiebeak/Food-Menu-AR/blob/master/GitDescription/ARMenu-Showcase.gif)

### AR
Self-made wrapper of internal `ImageLibrary` to easily set up data and bind it to the XR reference images. Custom `Tracker` to work with only one `TrackableImage` at time.  

### ADDRESSABLES
Addressables were set up in project and tested on local server. With them you can easily update asset data without the rebuilding of whole application. And manage different menus.

### EDITOR EXTENSIONS
The editor window allows you to bake a dish and it's ingredients with just one click.  
Of course it wouldn't be used in real life with dishes in restaurants, but it is very useful for me when I'm using simple 3D models of dishes with different objects.  
Primarily, this was developed to work with new UI Toolkit and Unity Editor.  
You can easily open it by `Services -> DishBaker` from the top toolbar.  

### UI TOOLKIT SCREEN SERVICE
A few simple screens were created using UI Toolkit and its data binding, USS animations and etc.  
I tried to divide the UI architecture into different layers via MVVM pattern to make it easier to replace the UI Toolkit with the UGUI in case. Maybe I overdid it a bit with its implementation. In any case, it can be simplified with the help of additional code generation.  
P.S. During the researching and learning some info about UI Toolkit, I came across an interesting [MVVM plugin](https://github.com/LibraStack/UnityMvvmToolkit/tree/main).  
