# CS253ShortestPath

## This project was my research project for my Data-structures class in college

---

## **Notes**
### The **Android project**:
  - You'll want to register/create your own API Key with Google Maps API, then:
    1. Change the API Key in the ```AndroidManifest``` file in the CS253ShortestPath.Android project to your new API Key
    2. Change the package name in the ```AndroidManifest``` file to your package name (or keep the same name and update the API Key)
    3. Make sure when creating your API Key to add your SHA1 debug hash to the Google Maps API

#### Android Project - Google Maps API Key + Xamarin Help
- [Using the Google Maps API in your application](https://docs.microsoft.com/en-us/xamarin/android/platform/maps-and-location/maps/maps-api)
- [Xamarin.Forms Map Initialization and Configuration](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/map/setup)
- [Obtaining a Google Maps API Key](https://docs.microsoft.com/en-us/xamarin/android/platform/maps-and-location/maps/obtaining-a-google-maps-api-key?tabs=windows)

### The **iOS project**:
- Please add an issue/issues if encountering build or deploy problems on iOS with VS for Mac or through XCode complaining about certificates. I will try to fix issues as soon as I can!

### The **Java project**?
- Yes yes, the Java project portion of this project will be added in the future; it is coded (99%) there; there are bugs in the Java side in the algorithm for calculating the shortest path. I will re-write the Java portion of the algorithm in C# and add it here as another top-level folder for the code.

---

## **Web Visualization Steps**
### In order to view the Web Marker visualization, please follow the steps below

1. From the app, export your saved Routes (there will be an export button on the ui, which you'll be able to send the json via email)
2. Copy and paste the ```exported-routes.json``` file from the email into the the same directory/folder as the ```webMarkerVis.py``` file  (**Note**: the filename must stay as is; if you change the filename, the change must be reflected in the ```webMarkerVis.py``` file so Python can find the json file)
3. Run the ```webMarkerVis.py``` file to generate the ```index.html``` file
4. Once the ```index.html``` file has been generated, open the file in your browser of choice and take a look at your routes and markers! :D