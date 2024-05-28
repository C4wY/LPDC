INCLUDE MIC_globals.ink

-> start
=== start ===
-> dialogue_check

=== dialogue_check ===
Player score is {playerScore}.
+ [Reset score] I will reset the score.
-> reset_score
+ [End dialogue] I will end the dialogue.
-> DONE

=== reset_score ===
~ playerScore = 0
Player score has been reset to {playerScore}.
-> dialogue_check
