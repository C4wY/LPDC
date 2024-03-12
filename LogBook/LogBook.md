# Journal de bord

## Follower demo
12/03/2024

### NavGraph + OneSided II
<video src="assets/Demo-NavGraph+OneSidedPlatform-2.mp4" controls width="640"></video>

Demo NavGraph + OneSidedPlatform (inclues dans navigation)
- OneSidedPlatform traversables en descente
- OneSidedPlatform prise en compte par le "nav graph" (via un layer dédié)

<br>

### NavGraph + OneSided
<video src="assets/Follower-Demo.mp4" controls width="640"></video>

Démo NavGraph / OneSidedPlatform (ignorées dans navigation)
- OneSidedPlatform sont ignorées lors de la génération du graph de navigation
- Les plateformes sont automatiquements mises à jour en fonction de la position 
  de l'avatar, mais il n'est pas possible de "descendre au travers".
