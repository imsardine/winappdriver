WinAppDriver - WebDriver for Windows Applications
=================================================

WinAppDriver is a [WebDriver implementation](//github.com/imsardine/winappdriver/wiki/Protocol-Implementation) for Windows applications, including [desktop applications](//github.com/imsardine/winappdriver/wiki/Desotop-Applications) and [universal apps](//github.com/imsardine/winappdriver/wiki/Universal-Apps) (formerly known as store apps, modern UI apps, or Metro-style apps). Support of [CEF-based desktop applications](//github.com/imsardine/winappdriver/wiki/Hybrid-Desktop-Applications) and [Windows Phone apps](//github.com/imsardine/winappdriver/wiki/Windows-Phone-Apps) is also planned.

Give it a try, and your feeback is appreciated.

##Getting Started

###Install WinAppDriver:

 1. [Download](https://github.com/imsardine/winappdriver/releases/download/v0.1/WinAppDriverInstaller.msi) the installer and execute it. A desktop shortcut will be created for launching _WinAppDriver Server_.

 2. Launch the server, and it listens on port 4444 on all addresses.

Then you can control the application under test (or even whole desktop) with any [WebDriver language bindings](http://docs.seleniumhq.org/download/#client-drivers) you prefer. Several [desired capabilities](//github.com/imsardine/winappdriver/wiki/Desired-Capabilities) could be used to how the server behaves for a specific session.

###Quick Start

Take whole desktop as an example:

(Python)
```python
from selenium.webdriver import Remote, DesiredCapabilities

desired_caps = {}
driver = Remote('http://your-winappdriver-server:4444/wd/hub', desired_caps)

driver.find_element_by_id('username').send_keys('your-username')
driver.find_element_by_id('password').send_keys('your-password')
driver.find_element_by_id('signin').click()
```

TBD: C#, Java, Ruby

Here are other scenarios supported by WinAppDriver:

 * [Whole Desktop](//github.com/imsardine/winappdriver/wiki/Whole-Desktop) (for more details)
 * [Desktop Applications](//github.com/imsardine/winappdriver/wiki/Desotop-Applications) ([already installed?](//github.com/imsardine/winappdriver/wiki/Desotop-Applications-Already-Installed))
 * [Universal Apps](//github.com/imsardine/winappdriver/wiki/Universal-Apps) ([already installed?](//github.com/imsardine/winappdriver/wiki/Universal-Apps-Already-Installed))

##Documentation

In addition to [wiki](//github.com/imsardine/winappdriver/wiki), Here are some documents/slides:

 * [Windows Store Apps Test Automation](http://www.slideshare.net/jeremykao92/winappdriver-windows-store-apps-test-automation)
 * [WinAppDriver Developemnt](http://www.slideshare.net/jeremykao92/winappdriver-development)

##License

WinAppDriver is licensed under MIT. Refer to [LICENSE](LICENSE) for more information.
