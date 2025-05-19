# CG-2025-1


## Entrega 1

### Mateo Jimenez Fama

Anexo el Gif de mi entrega:

![GifFinal](https://github.com/user-attachments/assets/6b205064-4847-47d2-a8d1-0001398c79fe)

### Carlos Santaella

Entrega Slash 

![EntregaSlash](https://github.com/user-attachments/assets/f3a92faf-ee84-4604-98fc-e47fc4b78e0e)

### Miguel Escobar

anexo gif de mi entrega:

![2025-02-28 03-25-09](https://github.com/user-attachments/assets/f913b827-37b2-42f8-b470-a6b18ea5f204)

## Entrega 2

### Miguel Escobar, Mateo Jimenez y Carlos Santaella

* GIF de la Entrega 2:

![AhoraSi](https://github.com/user-attachments/assets/18dd1ba3-776e-4667-856b-c52669d7e936)

## Entrega 3

### Carlos Santaella

1. Máscara Multicanal

   ![Multicanal](https://github.com/user-attachments/assets/3f88c087-5cd9-4182-83d5-737a609840ff)

2. Movimiento de Humo

Para la base texture utilicé una textura lo más parecida a humo, por eso se ve así xd

![Smoke](https://github.com/user-attachments/assets/b0fd9ae1-6a10-4b20-a2a9-5e3b309e5ce3)

3. Coordenadas Polares

![Polar](https://github.com/user-attachments/assets/069ac518-d0a5-4619-a702-b0a5925c89d9)

## Entrega 3

### Miguel Escobar

![2025-05-14 22-15-25](https://github.com/user-attachments/assets/ee5bec5e-8fb1-4d19-98d8-d77e1f8419c9)

## Entrega 3

### Mateo Jimenez Fama

1. Refraction

![image](https://github.com/user-attachments/assets/e7711c9d-b6e8-460f-824b-8b5b65894ccd)
![Gif Refraccion](https://github.com/user-attachments/assets/092f962e-018d-4e71-b34d-2d6327f80291)

2. Self Dissolve
   
   ![image](https://github.com/user-attachments/assets/aa045c83-317d-4dcc-92be-8b300925fc35)
   ![Gif SelfDissolve](https://github.com/user-attachments/assets/692b3f8f-fff9-440d-9d84-44a8a603722d)

4. MatCap

   ![image](https://github.com/user-attachments/assets/026495c0-a4c7-4a6f-9b76-288e8c1746ec)
   ![Gif Matcap](https://github.com/user-attachments/assets/798c689b-fe61-41ef-985e-67142d2ca724)


## Entrega 4 Grupal

### Grupo : Miguel Escobar, Carlos Santaella, Mateo Jimenez 

![ShaderAgua](https://github.com/user-attachments/assets/891201fd-e2c4-420d-b785-0bbd763bc2a2)

Explicacion detallada de cada parametro:

_Color: Color principal del agua cuando es poco profunda
_DepthColor: Color del agua en zonas profundas
_FoamColor: Color de la espuma que aparece en la superficie

Texturas

_NormalMap: Textura de normales que define el patrón de ondulaciones pequeñas
_NoiseTexture: Textura de ruido utilizada para efectos aleatorios
_FoamTexture: Textura que define el patrón de la espuma
_Cubemap: Mapa de cubo para las reflexiones en el agua

Parámetros de Ondas

_WaveAmplitude: Altura de las ondas en la superficie del agua
_WaveFrequency: Frecuencia de las ondas (cuántas ondas aparecen en un espacio)
_WaveSpeed: Velocidad a la que se mueven las ondas

Refracción y Reflexión

_RefractionStrength: Intensidad del efecto de refracción (distorsión del fondo)
_ReflectionStrength: Intensidad de las reflexiones del entorno
_Glossiness: Suavidad de la superficie (afecta los brillos especulares)
_FresnelPower: Potencia del efecto Fresnel que controla la visibilidad de las reflexiones según el ángulo de visión

Profundidad

_DepthDistance: Distancia a la que se mezcla el color superficial con el color profundo
_DepthFalloff: Rapidez con la que cambia el color según la profundidad

Espuma

_IntersectionFoamDepth: Distancia a la que aparece la espuma en intersecciones con objetos
_FoamCutoff: Umbral para determinar dónde aparece la espuma (valores más altos = menos espuma)
_FoamScale: Tamaño de los patrones de espuma
_WaveFoamCutoff: Umbral para la espuma que aparece en las crestas de las olas
