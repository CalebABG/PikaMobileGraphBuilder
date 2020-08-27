import os
import sys
import base64
import json
import folium
from folium import plugins

### Import JSON of exported Route data
dir_path = os.path.dirname(os.path.realpath(__file__))

json_data = None

try:
    with open('{}/{}'.format(dir_path, "exported-routes.json")) as f:
        json_data = json.load(f)
except() as exc:
    print("Exception: {}".format(exc))
    sys.exit(-1)

routes_array = json_data["routes"]
map_center_lat = float(json_data["map_center_lat"])
map_center_long = float(json_data["map_center_long"])

print("Total Saved Routes: {}".format(len(routes_array)))

### Create Map
f_map = folium.Map(
    location=[map_center_lat, map_center_long],
    zoom_start=16,
)

# m.add_child(folium.LatLngPopup())

### Make Markers for Map
route_points_count = 0
for route in routes_array:
    marker_list = json.loads(base64.b64decode(route["RoutePointsBlob"]))
    marker_locations = []

    for marker in marker_list:
        route_points_count += 1

        marker_id = marker["Id"]
        marker_title = "Id:{}".format(marker_id)

        marker_lat = marker["Latitude"]
        marker_long = marker["Longitude"]
        marker_locations.append([marker_lat, marker_long])

        marker_icon = plugins.BeautifyIcon(icon='arrow-down', icon_shape='marker', background_color=route["Color"])
        marker_popup = "<b>Id: {}</b> <br/><br/> <b>Lat: {}</b> <br/><br/> <b>Long: {}</b>".format(marker_id,
                                                                                                   marker_lat,
                                                                                                   marker_long)
        folium.Marker([marker_lat, marker_long],
                      popup=marker_popup,
                      tooltip=marker_title,
                      icon=marker_icon).add_to(f_map)

    danger_line = folium.PolyLine(
        marker_locations,
        weight=10,
        color=route["Color"],
        opacity=0.8).add_to(f_map)

print("Total Markers: {}".format(route_points_count))

### Save file
f_map.save("index.html")

print("Success! Generated index.html file")