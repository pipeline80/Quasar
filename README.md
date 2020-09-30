# Quasar
Bienvenido a la Operación Fuego de Quasar! :owlbert:

URL: https://quasar.azurewebsites.net/

# 📝 Ejecutar la API

Existen cuatro servicios

POST

https://quasar.azurewebsites.net/quasar/topsecret

SAMPLE REQUEST

POST /Todo
    [
        {
            "name": "Kenobi",
            "distance": 100,
            "message": ["este", "", "", "mensaje",""]
        },
        {
            "name": "Skywalker",
            "distance": 115.5,
            "message": ["", "es", "", "", "secreto"]
        },
        {
            "name": "Sato",
            "distance": 142.7,
            "message": ["este", "", "un", "mensaje", ""]
        }
    ]

GET
https://quasar.azurewebsites.net/quasar/topsecret_split

SAMPLE QUERYSTRING

?name=Kenobi&distance=100&message=este,es,un,,secreto

POST
https://quasar.azurewebsites.net/quasar/topsecret_split

SAMPLE REQUEST

POST /Todo
    [
        {
            "name": "Kenobi",
            "distance": 100,
            "message": ["este", "", "", "mensaje",""]
        },
        {
            "name": "Skywalker",
            "distance": 115.5,
            "message": ["", "es", "", "", "secreto"]
        }
    ]

POST
https://quasar.azurewebsites.net/quasar/topsecret_split


SAMPLE REQUEST

POST /Todo
    [
        {
            "distance": 100,
            "message": ["este", "", "", "mensaje",""]
        }
    ]

Mas información
https://quasar.azurewebsites.net/swagger/index.html

# 🚦 Configuración de Satélites

Para configurar la posición de los satélites se debe editar el archivo /Satellites.json

[
  {
    "name": "Kenobi",
    "position": {
      "positionX": -500,
      "positionY": -100
    }
  },
  {
    "name": "Skywalker",
    "position": {
      "positionX": 100,
      "positionY": -100
    }
  },
  {
    "name": "Sato",
    "position": {
      "positionX": 100,
      "positionY": 100
    }
  }
]
