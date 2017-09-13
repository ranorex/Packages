NeoLoad extension installed.

Quick start guide:

After installation you can drag NeoLoad modules from the Module Browser into your
test suite to communicate with, and control a NeoLoad server.

To make the project compile correctly you need to add the various binding redirects inside
the app.config.

A new app.config should look like this when the redirects were added:

<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <runtime>
        <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
            <dependentAssembly name="System.Runtime">
                <assemblyIdentity name="System.Runtime" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
                <bindingRedirect oldVersion="0.0.0.0-2.6.9.0" newVersion="2.6.9.0" />
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name="System.Threading.Tasks" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
                <bindingRedirect oldVersion="0.0.0.0-2.6.9.0" newVersion="2.6.9.0" />
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name="System.Net.Http" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
                <bindingRedirect oldVersion="0.0.0.0-2.2.28.0" newVersion="2.2.28.0" />
            </dependentAssembly>
        </assemblyBinding>
    </runtime>
</configuration>

For more information visit http://www.ranorex.com/support/user-guide-20/neoload.html
