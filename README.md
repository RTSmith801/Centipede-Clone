<!DOCTYPE html>
<html lang="en">

<head>
    <!--
    Centipede Clone
    Author: Rick Smith
    Date: December 2023 
    Project: RTSmith801/Centipede-Clone
    -->
    <meta charset="utf-8">
</head>

<body>
    <header>
        <a href="https://pixeltapestry.com/cv" target="_blank">
            <img src="https://pixeltapestry.com/wp-content/uploads/2023/09/Pixel_Tapestry_logo_yellow-e1694147529595.png" style="height: 50px; width: auto;" alt="Pixel Tapestry Logo">
        </a>
        <h1>Centipede Clone</h1>
        <p>This is a clone of Atari's Centipede © 1981. Created in Unity. I worked on this project Dec '22 - Jan '23.</p>
    </header>
    <main>
        <h2>Take Aways:</h2>
        <div>
            <h3>The largest challenges presented by this project</h3>
            <dl>
                <dt>Collision Detection and Handling</dt>
                <dd>Overcoming challenges related to accurate collision detection and response in a 2D game environment.</dd>
                <dt>Passing Back Information in a Linked List</dt>
                <dd>Implementing a linked list structure to manage and pass information, specifically the centipede’s segments and their behavior.</dd>
            </dl>
        </div>
        <div>
            <h3>What I learned from this project</h3>
            <dl>
                <dt>Using coroutines in the Game Manager</dt>
                <dd>Needing independent timers for game objects not yet instantiated required the use of coroutines to efficiently spawn enemies.</dd>
            </dl>
        </div>
        <div>
            <h3>How I would handle this differently if rebuilding this from scratch</h3>
            <dl>
                <dt>Player movement controls:</dt>
                <dd>Because the player movements do not require engine physics, creating colliders for the environment proved to be difficult while still maintaining smooth movements. If I were to recreate this project, I would like to start by examining different options for these controls. A raycast might provide a cleaner collision detection method.</dd>
            </dl>
        </div>
        <h2>Play This Game</h2>
        <p>This game can be played in browser <a href="https://pixeltapestry.com/resume/projects/centipede-clone/" target="_blank">here</a>.</p>
    </main>
</body>

</html>
