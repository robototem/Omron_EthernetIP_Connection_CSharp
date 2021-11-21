# Omron_EthernetIP_Connection_CSharp
This tool can be used, if you want to communicate with Omron PLCs using Ethernet IP via C#.net

# Important notes

1. Set the correct IP adress of the PLC.
2. Make sure that you share the same ethernet framework, and your IP must not overlap with PLC's IP
3. Write the correct start DM area to write bits
4. Write the correct start DM area to read bits
5. Thread will abort after closing the application.
6. OmronFINS is needed to be added as a reference.

# UI

![UI_Example](https://user-images.githubusercontent.com/84636881/142757975-32e5dc7a-2e5f-4228-a8db-9503cf28d2ce.jpg)
