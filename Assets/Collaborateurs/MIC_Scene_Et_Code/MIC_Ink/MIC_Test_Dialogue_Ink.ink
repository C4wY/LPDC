// MIC_Test_Dialogue_Ink.ink
INCLUDE MIC_globals.ink

=== start ===
Bonjour, aventurier !
Vous avez actuellement {playerScore} points.
Que voulez-vous faire ?

+ [Gagner des points]
    ~ playerScore += 10
    Félicitations ! Vous avez gagné 10 points.
    -> retour

+ [Perdre des points]
    ~ playerScore -= 5
    Dommage, vous avez perdu 5 points.
    -> retour

=== retour ===
Vous avez maintenant {playerScore} points.
Que voulez-vous faire ensuite ?

+ [Continuer] -> start
+ [Terminer]
    Au revoir, aventurier !
    -> DONE