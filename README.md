# A Situation Awareness Assisting HMI for Forestry Heavy Machines

(research paper link to be updated 😊)

> This project is a part of the research titled: 


## 🌟 Highlights

- Easy to control with a joystick
- Multi-screen settings
- Situation Awareness enhancing features: 2D Minimap & Viewer-Centered Panoramic view
- Embedded user study logics: conditions counter-balanced; night/day and easy/medium tasks auto switching; data collection


## ℹ️ Overview

This project is a part of a research conducted at [TAUCHI](https://research.tuni.fi/tauchi/), Tampere University, whose objective is to evaluate *how visual environmental information and different levels of drone autonomy affect navigation and identification performance*.

> General impression pic

It is a Unity project simulating the scenario where the user controlling a heavy machine in the forest. There will be a couple of different tasks and the user need to identify a nest in a specific tree to which a number is attached in each task.

> Nest and num pic

### Controls

- Joystick: constant speed
  - y axis: forward/backward
  - z axis: rotation
  - trigger button: drone takeoff for 2D minimap/panoramic view updates under the **Manual Condition**
  - pano view toggle button: toggling the pano view(please check the new input system inputs and their bindings for more info and any modification)
- Touch screen: user interface including number panel input etc..

### Procedure

To start with, you need to enter a participant id as well as an order number(integer from 1-6) indicating an arrangement of 3 different conditions: 
- Baseline: No assistance from 2D minimap or panoramic view
- Auto: You have access to the 2D minimap and pano view updating the environment info automatically
- Manual: You have access to the 2D minimap and pano view updated by triggering the drone at any location

### ✍️ Authors

Mention who you are and link to your GitHub or organization's website.


## 🚀 Usage

*Show off what your software looks like in action! Try to limit it to one-liners if possible and don't delve into API specifics.*

```py
>>> import mypackage
>>> mypackage.do_stuff()
'Oh yeah!'
```


## ⬇️ Installation

Simple, understandable installation instructions!

```bash
pip install my-package
```

And be sure to specify any other minimum requirements like Python versions or operating systems.

*You may be inclined to add development instructions here, don't.*


## 💭 Feedback and Contributing

Add a link to the Discussions tab in your repo and invite users to open issues for bugs/feature requests.

This is also a great place to invite others to contribute in any ways that make sense for your project. Point people to your DEVELOPMENT and/or CONTRIBUTING guides if you have them.
