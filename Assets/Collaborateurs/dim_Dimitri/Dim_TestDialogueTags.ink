Once upon a time... # speaker NPC
Que faire ? # speaker Dooms #choice Dooms Sora Neutral None

* [Dooms - Frapper] -> Frapper 
* [Sora - Séduire] -> Seduire 

== Frapper ==
Viens, on va leur casser la gueule #speaker Dooms 
OK... si tu le dis... #speaker Sora 
Tkt, fais moi confiance ! #speaker Dooms #portraitdooms angry
-> END

== Seduire ==
Bonsoir braves gens, vous etes resplendissant ce soir ! #speaker Sora #choice Sora Sora None None
* [Continuer] -> Continuer
* [Demander de Passer] -> DemanderPassage

== Continuer==
Je ne suis qu'un artiste de passage mais votre charisme m'inspire tant ! #speaker Sora
Tes jolis mots m'ont bien fait rire le mioche, je vais te laisser passer ! Tu es si ridicule que s'en est louable ! #speaker Brigand
-> END

==DemanderPassage==
Pourriez vous nous laisser passer s'il vous plait ? #speaker Sora
Dans tes rêves le nain ! Si tu veux passer faut payer... #speaker Brigand
C'est pas super ça... Pourquoi on devrait vous obeir ? #speaker Sora
On s'en fout ! On va juste leur casser les détruire ! #speaker Dooms #portraitdooms angry

-> END
