INCLUDE MIC_globals.ink

-> start2

=== start2 ===
Salut, aventurier !
Vous avez actuellement {playerScore} points.
Voulez-vous les réinitialiser à zéro ?

+ [Oui, réinitialiser à zéro]
    ~ playerScore = 0
    Vos points ont été réinitialisés.
    -> start2

+ [Non, conserver mes points]
    Très bien, continuez votre aventure !
    -> start2

+ [Terminer]
    Au revoir, aventurier !
    -> start2
