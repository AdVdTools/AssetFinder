Asset Finder v 1.13

Novedades:
+ Centra un GameObject en la vista de la escena haciendo doble click en la lista o pulsando F/Intro.
+ A�adido detalle a los GameObjects. Se muestra el icono correspondiente al componente que contiene el asset buscado.
+ Cambiada la selecci�n por rat�n. Un click selecciona el objeto, click derecho localiza el objeto en la jerarqu�a o el proyecto sin seleccionarlo.
+ Selecci�n multiple. Manten pulsado Shift para seleccionar una serie de objetos de la lista.
+ Flechas. Utiliza las flechas arriba y abajo para seleccionar objetos desplazandote por la lista. Utiliza las teclas derecha e izquierda para cambiar el modo de b�squeda.
+ 1-5. Utiliza los numeros del 1 al 5 para cambiar el tipo de asset a buscar.

Una vez hayas importado este paquete encontrar�s la ventana Asset Finder en el men� Window de Unity.

Asset Finder te permitir� encontrar los materiales, las texturas y las mallas que est�n usando los renderers de la escena actual. Podr�s filtrar la b�squeda de diversas formas y localizar aquellos objetos que est�n utilizando un asset concreto.

Barra de herramientas superior:
 - Selector del tipo de asset (1-5). Busca materiales, texturas o mallas. Tambi�n busca clips de audio o animaci�n.
 - Select All. Selecciona todos los objetos del filtro. Si se pulsa Ctrl/Cmd al mismo tiempo se a�adir� la busqueda a la selecci�n actual.
 - Men� de opciones: (Es posible abrir un men� de opciones extendidos haciendo clic derecho sobre la ventana)
   � Sort by Name. Ordena el resultado de la b�squeda por orden alfab�tico.
   � Ignore Inactive. S�lo se buscar� en aquellos GameObjects que est�n activos en la jerarqu�a.
   � Search in Selection Only. S�lo se buscar� en los objetos seleccionados y sus hijos.
   � Show Detail. Muestra detalles de los objetos de la lista.
 - Modo de b�squeda:
   � Find Assets. Busca materiales, texturas o mallas usados en GameObjects.
   � Find Users (F). Busca GameObjects que usen un material, textura o malla concretos.


Opciones de filtrado:
 - Shader name. S�lo se buscar�n materiales o texturas que se encuentren en materiales cuyo nombre empiece como 'Shader name'.
 - Material property name. S�lo se buscar�n texturas en la propiedad 'Material property name' de cada material encontrado.
 - MeshFilters/MeshColliders. Elige buscar mallas en un tipo de componente concreto o en ambos.
 - Material/Texture/Mesh (Find Users). Selecciona el asset por el que filtrar la busqueda de GameObjects.
 - Filter. Filtra la b�squeda de objetos o assets que contengan 'Filter' en su nombre.


Resultado de la b�squeda:
 - Haz click derecho sobre cualquier elemento de la lista para mostrar su ubicaci�n en la jerarqu�a o el proyecto.
 - Haz click izquierdo sobre cualquier elemento para seleccionarlo.
 - Haz doble click (o F/Intro) sobre un asset para buscar sus usuarios, o sobre un GameObject para centrarlo en la vista de la escena.
 - Utiliza las flechas arriba y abajo para desplazarte por la lista.
 - Haz click mientras mantienes Ctrl/Cmd para a�adir o quitar un elemento de la selecci�n.
 - Mant�n pulsado Shift para seleccionar una serie de elementos de la lista.
 - Se muestran detalles de los assets encontrados junto a su nombre. Algunos tipos de asset permiten alternar el detalle mostrado haciendo clic sobre este.
 - Find Users. Haz click sobre la lupa para buscar los GameObjects que utilizan un asset concreto. Tambi�n puedes arrastrar el asset sobre la ventana.


Notas:
 - S�lo se encontrar�n materiales en los Renderers que superen los filtros.
 - S�lo se encontrar�n texturas en los materiales que se encuentren.
 - S�lo se encontrar�n mallas en el componente MeshFilter o el componente MeshRenderer.
 - La lista de shaders s�lo muestra aquellos shaders que se hayan cargado en la sesi�n de Unity.
 - La lista de propiedades s�lo muestra las propiedades de texturas del shader actual o los shaders m�s comunes.
 - Los resultados de la b�squeda no se actualizan ante cualquier cambio y es posible que tengas que actualizar manualmente. Para ello simplemente haz click sobre la ventana.
 - Actualmente no se pueden encontrar texturas en elementos de UI o en SpriteRenderers.
 - Solo encuentra AudioClips en componentes AudioSource.
 - No se soporta la b�squeda de AnimationClips Legacy en componentes Animation, s�lo en Animator.
 - Unity permite buscar referencias a un objeto en la escena (Click derecho sobre un asset -> Find References In Scene) lo que permite buscar usuarios donde AssetFinder no llega.