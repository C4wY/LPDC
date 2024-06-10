INCLUDE MIC_globals.ink

-> start
=== start ===
-> dialogue_intro

=== dialogue_intro ===
+ [Increase score] I will increase the score.
-> increase_score
+ [End dialogue] I will end the dialogue.
-> DONE

=== increase_score ===
~ playerScore = playerScore + 1
Player score is now {playerScore}.
-> dialogue_intro
