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
  > baseline pic
- Auto: You have access to the 2D minimap and pano view updating the environment info automatically
  > auto pic
- Manual: You have access to the 2D minimap and pano view updated by triggering the drone at any location
  > man pic

After clicking the start button from the start page, you will jump directly to the first task where you need to roam around the forest following the direction arrow on the minimap to find the target number. There will be 3 * 4 = 12 tasks in total.

### ✍️ Authors

[My Github account](https://github.com/Ed-Tivrusky-IV)

[My LinkedIn Profile](https://www.linkedin.com/in/jaime-li-yuancao/)

[Iqbal](https://github.com/adbaga) contributed to the number panel logic and data collection related scripts.


## 🚀 Usage

You might need to add glTFast package from Unity package manager if the .glb model is not rendered properly.

You might change the bindings of the input from Unity input manager.

You also need to map the screens(4 screens in total) with the rendering cameras.


## ⬇️ Installation

Clone the project and open it with your Unity Hub.

Unity Version 6.1 is recommended.


## 💭 Feedback and Contributing

[Contact me](mailto:yuancaojl@outlook.com) if you have any questions/potential collaboration😊
