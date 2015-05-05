WinAppDriver - WebDriver for Windows Applications
=================================================

WinAppDriver is a WebDriver implementation for Windows applications, including desktop applications and store apps (formerly known as Metro-style apps). Support of CEF-based desktop applications and Windows Phone apps is also planned.

##Getting Started

###Install WinAppDriver:

 1. [Download](https://github.com/imsardine/winappdriver/releases/download/v0.1/WinAppDriverInstaller.msi) the installer and execute it.

 2. The installer creates a shortcut named _WinAppDriver_. Use the shortcut to launch _WinAppDriver server_, which listens on port 4444 on all addresses.

###Not to target a specific app: (whole desktop)

```python
from selenium.webdriver import Remote, DesiredCapabilities

desired_caps = {}
driver = Remote('http://your-winappdriver-server:4444/wd/hub', desired_caps)
```

###To target a specific store app:

```python
from selenium.webdriver import Remote, DesiredCapabilities

desired_caps = {
    'platformName': 'WindowsModern',
    'app': 'http://your-ci-server/path/to/your/pacakge.zip',
    'packageName': 'your-app-package-name',
}

driver = Remote('http://your-winappdriver-server:4444/wd/hub', desired_caps)
```

###To target a specific desktop app:

```python
from selenium.webdriver import Remote, DesiredCapabilities

desired_caps = {
    #'platformName': 'Windows',
    'appID': 'your-product-name',
    'app': 'http://your-ci-server/path/to/your/installer.exe',
    'checkInstalledCommand': r'C:\path\to\your\check-installed.bat',
    'openCommand': r'C:\path\to\your\open-app.bat',
    'closeCommand': r'C:\path\to\your\close-app-if-needed.bat',
    'installCommand': r'C:\path\to\your\install-if-needed.bat',
    'uninstallCommand': r'C:\path\to\your\uninstall-if-needed.bat',
    'backupCommand': r'C:\path\to\your\backup-states-if-needed.bat',
    'restoreCommand': r'C:\path\to\your\restore-states-if-needed.bat',
}

driver = Remote('http://your-winappdriver-server:4444/wd/hub', desired_caps)
```

##Status

Support for desktop applications and store apps is almost done. Give it a try, and your feeback is appreciated.

Here are some documents for your reference as well:

 * For an overview of what it is, refer to [Windows Store Apps Test Automation](http://www.slideshare.net/jeremykao92/winappdriver-windows-store-apps-test-automation).
 * For how to contribute, refer to [WinAppDriver Developemnt](http://www.slideshare.net/jeremykao92/winappdriver-development).

##License

WinAppDriver is licensed under MIT. Refer to [LICENSE](LICENSE) for more information.
