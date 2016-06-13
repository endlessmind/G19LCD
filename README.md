# Logitech G19 Display .Net library - Low level control

This library let's you control the display on a Logitech G19 via the LibUsb driver.
It gives you full control over what is shown on the display.


Test layout! (Included in the samples)

![alt tag](https://raw.githubusercontent.com/endlessmind/G19LCD/master/page1.jpg)
# Why and how?

This project was made to let you use the display stand-alone without the rest of the keyboard,
but will also work with the keyboard. Marco-buttons will NOT work, as they operate on the same driver as the display.

</br>

With this library you could also attach a touchscreen overlay to the display as long as you implement support for that in 
your code. The screen-assembly is connected to the built in usb-hub controller on the right side of the G19 keyboard, but would 
work without it if you connect a USB-cable directly to the screen-assembly.

</br>

I do recommend keeping the usb-hub if possible, in case you want to add touchscreen, as touchscreens also operates via usb.




# Dependencies
 
 
<ul>
	<li>LibUsb driver must be installed on the Logitech G19 Gaming Keyboard or Logitech LCD Interface
	
		</br>
		
		(G19: Vendor ID: 046D, Product ID: C229)
	</li>
	<li> .Net Framework 3.5</li>
</ul>
 

</br>
 
</br>

</br>


Make sure LCore.exe is not running before trying to use the library (May not be needed, everything works fine when I forgot to cloes LCore.exe)

You can download the driver wizard (infWizard) from <a href="http://libusbdotnet.sourceforge.net/">LibUsbDotNet's page</a>