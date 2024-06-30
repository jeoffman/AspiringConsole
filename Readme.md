# running just the aspire dashboard from a command line program
Trying so hard to get the Aspire Dashboard running standalone AND hosted by another app (Winforms, Console, etc.)

# Jkh.AspireDashboardHosting
Stolen from https://github.com/martinjt/aspire-app-extension and tweaked to work
Some of the dashboard parameters are pretty generic, but it works for me and YMMV

# ConsoleApp1
Notice the .csproj is <Project Sdk="Microsoft.NET.Sdk.web">


1) Just run the console (F5 Debug)
2) Point your browser to http://localhost:9999 to see the aspire dashboard GUI
3) Run any other app that logs to OTLP on localhost 4317 (the default) to log something

WARN: The "publish" doesn't quite work right - some kind of Blazor error inside the dashboard causes the logs to 
stop showing up. Works in the debug build, so maybe its some kind of production-thing?
