*This package requires Unity 2021.3 or later to use.*

Thank you for checking out the trial version of my Tree Viewer! 
Tree Viewer is a tool designed to help you visualize tree-like systems, such as hierarchal node machines. 
This package contains all necessary files to use the tool as well as some samples if you get stuck.

// HOW TO USE /////////////////////////////////////////////
1. Implement the ITreeViewer interface on the root Tree gameobject.
2. Implement the ITreeNodeViewer interface on your base node class
3. Select "Tools/Tree Viewer" in the menu bar to open the window.
	- Note that a default Test Tree opens with the window.
4. Click on your root Tree gameobject in the hierarchy to see it in the Tree Viewer window.
	- Note that if your Tree is intialized at runtime that it won't appear unless you're in play mode.
	- Note that if there are multiple Trees of the same type in a scene, clicking each will show that instance's current node.

// LAYOUTS ////////////////////////////////////////////////
The default view for the Tree is not a one size fits all. Blocks could overlap, or you may not like the default view for that Tree. 
That's why you can hide child nodes, drag blocks around, and save these changes in the Layouts menu in the Tree Viewer toolbar.
Saved layouts are unique to the Tree type but not the Tree instance. This is best illustrated in the "Hierachal State Machine" sample. 

// SETTINGS AND MORE //////////////////////////////////////
The Tree Viewer toolbar has a settings menu for you to customize the look of your window. These changes are instantly saved and are constant across all Trees.

The node blocks have context menus. You can right click them to re-organize all nodes below them to the default positions.

If you press "R" while focused on the editor window, it will reset the window to your currently saved layout, if one exists. Otherwise it goes to the default.
If you press "F" while focused on the editor window, it will bring the camera back to the origin, at the default zoom.