TalkBack - Project Overview

TalkBack is a client-server messaging system that enables users to sign in, manage contacts, engage in real-time text chatting, and play backgammon against each other.

Key Requirements & Architecture
●	System Architecture:
○	Consists of a Windows-based client application (originally built for .NET), an application server configured via XML/INI files, and a database storing user credentials and message history.
●	Presentation Layer (GUI):
○	Sign-in Page: Authenticates users via username and password, with support for remembering usernames.
○	Contact Pane: Displays online and offline contacts, enabling quick access to chat or backgammon game invitations.
○	Chat Pane: Features a message history display and a text composing area with keyboard support.
○	Game Pane: A full backgammon board interface supporting dice rolling, piece movement governed by game rules, a 2--minute turn timer, move undo options, and resignation handling.
●	Application Layer & Server:
○	The server manages user status (online/offline), routes chat messages in real-time, and handles game requests, acceptances, and coordination.
●	Non-Functional Requirements:
○	Client requests to the application server must be processed within 5 seconds for ADSL/cable connections and up to 10 seconds for dial-up.
