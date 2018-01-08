Asset Finder v 1.13

Novedades:
+ Centra un GameObject en la vista de la escena haciendo doble click en la lista o pulsando F/Intro.
+ Añadido detalle a los GameObjects. Se muestra el icono correspondiente al componente que contiene el asset buscado.
+ Cambiada la selección por ratón. Un click selecciona el objeto, click derecho localiza el objeto en la jerarquía o el proyecto sin seleccionarlo.
+ Selección multiple. Manten pulsado Shift para seleccionar una serie de objetos de la lista.
+ Flechas. Utiliza las flechas arriba y abajo para seleccionar objetos desplazandote por la lista. Utiliza las teclas derecha e izquierda para cambiar el modo de búsqueda.
+ 1-5. Utiliza los numeros del 1 al 5 para cambiar el tipo de asset a buscar.

Una vez hayas importado este paquete encontrarás la ventana Asset Finder en el menú Window de Unity.

Asset Finder te permitirá encontrar los materiales, las texturas y las mallas que están usando los renderers de la escena actual. Podrás filtrar la búsqueda de diversas formas y localizar aquellos objetos que están utilizando un asset concreto.

Barra de herramientas superior:
 - Selector del tipo de asset (1-5). Busca materiales, texturas o mallas. También busca clips de audio o animación.
 - Select All. Selecciona todos los objetos del filtro. Si se pulsa Ctrl/Cmd al mismo tiempo se añadirá la busqueda a la selección actual.
 - Menú de opciones: (Es posible abrir un menú de opciones extendidos haciendo clic derecho sobre la ventana)
   · Sort by Name. Ordena el resultado de la búsqueda por orden alfabético.
   · Ignore Inactive. Sólo se buscará en aquellos GameObjects que estén activos en la jerarquía.
   · Search in Selection Only. Sólo se buscará en los objetos seleccionados y sus hijos.
   · Show Detail. Muestra detalles de los objetos de la lista.
 - Modo de búsqueda:
   · Find Assets. Busca materiales, texturas o mallas usados en GameObjects.
   · Find Users (F). Busca GameObjects que usen un material, textura o malla concretos.


Opciones de filtrado:
 - Shader name. Sólo se buscarán materiales o texturas que se encuentren en materiales cuyo nombre empiece como 'Shader name'.
 - Material property name. Sólo se buscarán texturas en la propiedad 'Material property name' de cada material encontrado.
 - MeshFilters/MeshColliders. Elige buscar mallas en un tipo de componente concreto o en ambos.
 - Material/Texture/Mesh (Find Users). Selecciona el asset por el que filtrar la busqueda de GameObjects.
 - Filter. Filtra la búsqueda de objetos o assets que contengan 'Filter' en su nombre.


Resultado de la búsqueda:
 - Haz click derecho sobre cualquier elemento de la lista para mostrar su ubicación en la jerarquía o el proyecto.
 - Haz click izquierdo sobre cualquier elemento para seleccionarlo.
 - Haz doble click (o F/Intro) sobre un asset para buscar sus usuarios, o sobre un GameObject para centrarlo en la vista de la escena.
 - Utiliza las flechas arriba y abajo para desplazarte por la lista.
 - Haz click mientras mantienes Ctrl/Cmd para añadir o quitar un elemento de la selección.
 - Mantén pulsado Shift para seleccionar una serie de elementos de la lista.
 - Se muestran detalles de los assets encontrados junto a su nombre. Algunos tipos de asset permiten alternar el detalle mostrado haciendo clic sobre este.
 - Find Users. Haz click sobre la lupa para buscar los GameObjects que utilizan un asset concreto. También puedes arrastrar el asset sobre la ventana.


Notas:
 - Sólo se encontrarán materiales en los Renderers que superen los filtros.
 - Sólo se encontrarán texturas en los materiales que se encuentren.
 - Sólo se encontrarán mallas en el componente MeshFilter o el componente MeshRenderer.
 - La lista de shaders sólo muestra aquellos shaders que se hayan cargado en la sesión de Unity.
 - La lista de propiedades sólo muestra las propiedades de texturas del shader actual o los shaders más comunes.
 - Los resultados de la búsqueda no se actualizan ante cualquier cambio y es posible que tengas que actualizar manualmente. Para ello simplemente haz click sobre la ventana.
 - Actualmente no se pueden encontrar texturas en elementos de UI o en SpriteRenderers.
 - Solo encuentra AudioClips en componentes AudioSource.
 - No se soporta la búsqueda de AnimationClips Legacy en componentes Animation, sólo en Animator.
 - Unity permite buscar referencias a un objeto en la escena (Click derecho sobre un asset -> Find References In Scene) lo que permite buscar usuarios donde AssetFinder no llega.