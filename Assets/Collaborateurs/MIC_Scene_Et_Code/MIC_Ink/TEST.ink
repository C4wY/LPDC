INCLUDE MIC_globals.ink

-> start

=== start ===
Bonjour, aventurier !
Vous avez actuellement {playerScore} points.
Que voulez-vous faire ?

+ [Gagner des points]
    ~ playerScore += 10
    Félicitations ! Vous avez gagné 10 points.
    -> start

+ [Perdre des points]
    ~ playerScore -= 5
    Dommage, vous avez perdu 5 points.
    -> start

+ [Terminer]
    Au revoir, aventurier !
    -> start