# Weather Loupedeck Plugin
[![License](http://img.shields.io/:license-MIT-blue.svg?style=flat)](LICENSE)
![forks](https://img.shields.io/github/forks/Steinerd/Loupedeck.WeatherPlugin?style=flat)
![stars](https://img.shields.io/github/stars/Steinerd/Loupedeck.WeatherPlugin?style=flat)
![issues](https://img.shields.io/github/issues/Steinerd/Loupedeck.WeatherPlugin?style=flat)
[![downloads](https://img.shields.io/github/downloads/Steinerd/Loupedeck.WeatherPlugin/latest/total?style=flat)](https://github.com/Steinerd/Loupedeck.WeatherPlugin/compare)


## Credits

This plugin is *heavily* influenced by [CZDanol/loupedeck-weatherwidget](https://github.com/CZDanol/loupedeck-weatherwidget). 
I personally couldn't get it to work, and decided to create a smaller, leaner version of the same concepts. 

--------

# Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Support](#support)
- [Contributing](#contributing)
- [License](#license)

# Installation

<details><summary><b>Loupedeck Installation</b></summary>
  
  
  1. Go to [latest release](https://github.com/Steinerd/Loupedeck.WeatherPlugin/releases/latest), and download the `lplug4` file to you computer
  1. Open (normally double-click) to install, the Loupedeck software should take care of the rest
  1. Restart Loupedeck (if not handled by the installer)
  1. In the Loupedeck interface, enable **Weather** by clicking <ins>Manage plugins</ins>
  1. Check the Weather box on to enable
  1. Drag the desired control onto your layout

Once click it will bring you to a dynamic playback device selection page. 
</details>

<details><summary><b>IDE Installation</b></summary>
  Made with Visual Studio 2022, C# will likely only compile in VS2019 or greater. 

  Assuming Loupedeck is already installed on your machine, make sure you've stopped it before you debug the project. 

  Debugging _should_ build the solution, which will then output the DLL, config, and pdb into your `%LocalAppData%\Loupedeck\Plugins` directory.

  If all goes well, Loupedeck will then open and you can then debug. 

</details>

# Usage

First, follow the __Loupedeck Installation__ instructions above. 

Then, to use inside the Loupedeck UI: 

1. Go to https://www.weatherapi.com/signup.aspx and create a free account, if you don't have one already
1. Api Key should be provided at the top of the page
1. Click the [+] button on the same row	as *Location* to add a location
1. Now comes the fun part... the value for this control is colon delimited; `location_query:weatherapi_key:hide_name` (ex: `90210:64e546s54546546f5da:true` to show weather for Beverly Hills and hide the name in the widget ).
	1. Enter a city name, or zip/postal code ([anything in the example highlighted here](https://www.weatherapi.com/docs/#:~:text=q,could%20be%20following%3A))
	1. :
	1. API Key
	1. : 
	1. true/false
1. Select Hook from the final dropdown and click Save

<details>summary><b>Advanced Users</b></summary>
I've added an additional nicety of having pressing the widget and having it open an application on the URL Protocol `weather:LocationQuery`.

So if you look in your registry `HKEY_CLASSES_ROOT\weather\shell\open\command` you'll see a crappy placeholder for a CMD prompt running weather.com passing in the first segment from the
the weather action configuration. 

You can replace it with your favorite browser explicitly with the `%1` being that location-query item. 

I personally replaced mine with the Win10 Weather App, `"C:\Windows\explorer.exe" shell:AppsFolder\Microsoft.BingWeather_8wekyb3d8bbwe!App`. 

You can also just drop the `HKEY_CLASSES_ROOT\weather` key if you don't want it to do anything for any of them
</details>

# Support

[Submit an issue](https://github.com/Steinerd/Loupedeck.WeatherPlugin/issues/new)

Fill out the template to the best of your abilities and send it through. 

# Contribute

Easily done. Just [open a pull request](https://github.com/Steinerd/Loupedeck.WeatherPlugin/compare). 

Don't worry about specifics, I'll handle the minutia. 

# License
The MIT-License for this plugin can be reviewed at [LICENSE](LICENSE) attached to this repo.
