# Logitech G19 Display .Net library - Low level control

This library let's you control the display on a Logitech G19 via the LibUsb driver.
It gives you full control over what is shown on the display.


# Why and how?

This project was made to let you use the display stand-alone without the rest of the keyboard,
but will also work with the keyboard. Marco-buttons will NOT work, as they operate on the same driver as the display.

Still library will also allow you do attach a touchscreen overlay to the display as long as you implement support for that in 
your code. The screen-assembly is connected to the built in usb-hub controller on the right side of the G19 keyboard, but would 
work without it if you connect a USB-cable directly to the screen-assembly.

I do recommend keeping the usb-hub if possible, in case you want to add touchscreen, as touchscreens also operates via usb.




# Dependencies
 LibUsb driver must be installed on the Logitech G19 Gaming Keyboard or Logitech LCD Interface
 
 (G19: Vendor ID: 046D, Product ID: C229)
 

 
 .Net Framework 3.5


 
 




Make sure LCore.exe is not running before trying to use the library