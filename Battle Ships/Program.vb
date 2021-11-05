Imports System.IO

Module Program
    Dim playerBoard(8, 8), enemyBoard(8, 8), playerName As String
    Dim playerMissilesFired, playerMissilesHit, enemyMissilesFired, enemyMissilesHit, turnID As Integer
    Dim loadedGame As Boolean = False
    Dim playerWon, enemyWon As Boolean

    Sub Main()
        Dim choice As String

        Console.ForegroundColor = ConsoleColor.White

        MainMenu()

        Do
            Console.WriteLine()
            Console.WriteLine("[1] Play Again")
            Console.WriteLine("[2] Exit To Menu")
            Console.WriteLine("[3] Exit Game")
            Console.Write("Choice > ")
            choice = Console.ReadLine

            If choice = "1" Then
                loadedGame = False
                PlayGame()
            ElseIf choice = "2" Then
                Console.Clear()
                MainMenu()
            ElseIf choice = "3" Then
                End
            Else
                Console.WriteLine("Invalid Choice! Please type '1', '2' or '3'.")
                Console.WriteLine()
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine("Press Enter to continue")
                Console.ForegroundColor = ConsoleColor.White
                Console.ReadLine()
                Console.Clear()
            End If
        Loop
    End Sub

    Sub MainMenu()
        Dim choice As String

        Do
            Console.WriteLine("[1] Start Game")
            Console.WriteLine("[2] Load Game")
            Console.WriteLine("[3] Exit Game")
            Console.Write("Choice > ")
            choice = Console.ReadLine

            If choice = "1" Then
                loadedGame = False
                PlayGame()
            ElseIf choice = "2" Then
                LoadGame()
            ElseIf choice = "3" Then
                End
            Else
                Console.WriteLine("Invalid Choice! Please type '1', '2' or '3'.")
                Console.WriteLine()
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine("Press Enter to continue")
                Console.ForegroundColor = ConsoleColor.White
                Console.ReadLine()
                Console.Clear()
            End If
        Loop Until choice = "1" Or choice = "2" Or choice = "3"
    End Sub

    Sub PlayGame()
        Dim randomNum As New Random

        If Not loadedGame Then
            GenerateBoard(playerBoard)
            GenerateBoard(enemyBoard)
            playerMissilesFired = 0
            playerMissilesHit = 0
            enemyMissilesFired = 0
            enemyMissilesHit = 0
            turnID = 0

            Console.Clear()
            Console.Write("Enter your name > ")
            playerName = Console.ReadLine
            Console.ReadLine()
        End If
        playerWon = False
        enemyWon = False

        Console.WriteLine("YOUR BOARD")
        DisplayBoard(playerBoard, 0)
        Console.WriteLine()

        Do
            PlayerTurn()

            If CheckBoard(enemyBoard) Then
                playerWon = True
                Exit Do
            End If

            EasyEnemyTurn()

            If CheckBoard(playerBoard) Then
                enemyWon = True
                Exit Do
            End If

            Console.Clear()
        Loop Until playerWon Or enemyWon ' conditions not strictly required
        Console.Clear()

        If playerWon Then
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("╔══════════╗")
            Console.WriteLine("║ YOU WON! ║")
            Console.WriteLine("╚══════════╝")
            Console.ForegroundColor = ConsoleColor.White
        ElseIf enemyWon Then
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("╔═══════════╗")
            Console.WriteLine("║ YOU LOST! ║")
            Console.WriteLine("╚═══════════╝")
            Console.ForegroundColor = ConsoleColor.White
        End If

        Console.WriteLine()
        Console.WriteLine("STATS")
        Console.ForegroundColor = ConsoleColor.Blue
        Console.Write("YOU ")
        Console.ForegroundColor = ConsoleColor.White
        Console.Write("fired ")
        Console.ForegroundColor = ConsoleColor.Blue
        Console.Write(playerMissilesFired)
        Console.ForegroundColor = ConsoleColor.White
        Console.Write(" missiles, out of which ")
        Console.ForegroundColor = ConsoleColor.Blue
        Console.Write(playerMissilesHit)
        Console.ForegroundColor = ConsoleColor.White
        Console.Write(" hit an enemy ship for an accuracy of ")
        Console.ForegroundColor = ConsoleColor.Blue
        Console.Write($"{Math.Round(playerMissilesHit / playerMissilesFired * 100, 2)}%")
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine("!")

        Console.ForegroundColor = ConsoleColor.Red
        Console.Write("THE ENEMY ")
        Console.ForegroundColor = ConsoleColor.White
        Console.Write("fired ")
        Console.ForegroundColor = ConsoleColor.Red
        Console.Write(enemyMissilesFired)
        Console.ForegroundColor = ConsoleColor.White
        Console.Write(" missiles, out of which ")
        Console.ForegroundColor = ConsoleColor.Red
        Console.Write(enemyMissilesHit)
        Console.ForegroundColor = ConsoleColor.White
        Console.Write(" hit a player ship for an accuracy of ")
        Console.ForegroundColor = ConsoleColor.Red
        Console.Write($"{Math.Round(enemyMissilesHit / enemyMissilesFired * 100, 2)}%")
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine("!")

        Console.WriteLine()
        TopScores()
    End Sub

    Function CheckBoard(gameBoard)
        For y = 0 To gameBoard.GetLength(0) - 1
            For x = 0 To gameBoard.GetLength(1) - 1
                If gameBoard(x, y).IndexOf("S") <> -1 And gameBoard(x, y).IndexOf("X") = -1 Then
                    Return False
                Else
                    Continue For
                End If
            Next
        Next

        Return True
    End Function

    Function ShipsLeft(gameBoard)
        Dim numOfShipsLeft As Integer
        numOfShipsLeft = 0

        For y = 0 To gameBoard.GetLength(0) - 1
            For x = 0 To gameBoard.GetLength(1) - 1
                If gameBoard(x, y).IndexOf("S") <> -1 And gameBoard(x, y).IndexOf("X") = -1 Then
                    numOfShipsLeft += 1
                End If
            Next
        Next

        Return numOfShipsLeft
    End Function

    Sub TopScores()
        ' Top 5 Scores Leaderboard
        Dim fileReader As StreamReader
        Dim fileWriter As StreamWriter
        Dim filename As String
        Dim scores(1, 4) As String

        filename = "TopScores.txt"
        Try
            fileReader = New StreamReader(filename)
        Catch ex As Exception
            fileWriter = New StreamWriter(filename)

            For r = 0 To 4
                For c = 0 To 1
                    If c = 0 Then
                        fileWriter.WriteLine("999")
                    Else
                        fileWriter.WriteLine("PLACEHOLDER")
                    End If
                Next
            Next

            fileWriter.Close()

            fileReader = New StreamReader(filename)
        End Try

        For r = 0 To 4
            For c = 0 To 1
                scores(c, r) = fileReader.ReadLine
            Next
        Next
        fileReader.Close()

        If playerWon Then
            For r = 0 To 4
                If playerMissilesFired < CInt(scores(0, r)) Then
                    scores(0, r) = playerMissilesFired.ToString
                    scores(1, r) = playerName
                    Exit For
                End If
            Next
        End If

        Console.WriteLine("TOP SCORES")
        For r = 0 To 4
            For c = 0 To 1
                If c = 0 Then
                    Console.Write($"{scores(c, r)} - ")
                Else
                    Console.WriteLine(scores(c, r))
                End If
            Next
        Next

        fileWriter = New StreamWriter(filename)
        For r = 0 To 4
            For c = 0 To 1
                fileWriter.WriteLine(scores(c, r))
            Next
        Next
        fileWriter.Close()
    End Sub

    Sub SaveGame()
        Dim fileWriter As StreamWriter
        Dim filename As String

        filename = "PlayerBoard.txt"
        fileWriter = New StreamWriter(filename)
        For y = 0 To playerBoard.GetLength(0) - 1
            For x = 0 To playerBoard.GetLength(1) - 1
                fileWriter.WriteLine(playerBoard(x, y))
            Next
        Next
        fileWriter.Close()

        filename = "EnemyBoard.txt"
        fileWriter = New StreamWriter(filename)
        For y = 0 To enemyBoard.GetLength(0) - 1
            For x = 0 To enemyBoard.GetLength(1) - 1
                fileWriter.WriteLine(enemyBoard(x, y))
            Next
        Next
        fileWriter.Close()

        ' There is probably a better way to do this
        filename = "GameStats.txt"
        fileWriter = New StreamWriter(filename)
        fileWriter.WriteLine(playerName)
        fileWriter.WriteLine(playerMissilesFired)
        fileWriter.WriteLine(playerMissilesHit)
        fileWriter.WriteLine(enemyMissilesFired)
        fileWriter.WriteLine(enemyMissilesHit)
        fileWriter.Close()
    End Sub

    Sub LoadGame()
        Dim fileReader As StreamReader
        Dim filename As String

        filename = "PlayerBoard.txt"
        Try
            fileReader = New StreamReader(filename)
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine()
            Console.WriteLine("No saved games found!")
            Console.ForegroundColor = ConsoleColor.DarkGray
            Console.WriteLine()
            Console.WriteLine("Press Enter to continue")
            Console.ForegroundColor = ConsoleColor.White
            Console.ReadLine()

            Console.Clear()
            MainMenu()
            Exit Sub
        End Try

        For y = 0 To playerBoard.GetLength(0) - 1
            For x = 0 To playerBoard.GetLength(1) - 1
                playerBoard(x, y) = fileReader.ReadLine
            Next
        Next

        filename = "EnemyBoard.txt"
        fileReader = New StreamReader(filename)
        For y = 0 To enemyBoard.GetLength(0) - 1
            For x = 0 To enemyBoard.GetLength(0) - 1
                enemyBoard(x, y) = fileReader.ReadLine
            Next
        Next
        fileReader.Close()

        ' Again, there is likely a better way to do this
        filename = "GameStats.txt"
        fileReader = New StreamReader(filename)
        playerName = fileReader.ReadLine
        playerMissilesFired = fileReader.ReadLine
        playerMissilesHit = fileReader.ReadLine
        enemyMissilesFired = fileReader.ReadLine
        enemyMissilesHit = fileReader.ReadLine
        fileReader.Close()

        loadedGame = True
        PlayGame()
    End Sub

    Sub HardEnemyTurn()
        Dim validMissileLocation As Boolean
        Dim missileLocationX, missileLocationY As Integer
        Dim randomNum As New Random

        Console.Clear()
        Console.WriteLine("Enemy is firing a missile...")
        Console.WriteLine()
        Threading.Thread.Sleep(1000)

        ' ToDo: Improved AI
    End Sub

    Sub EasyEnemyTurn()
        Dim validMissileLocation As Boolean
        Dim missileLocationX, missileLocationY As Integer
        Dim randomNum As New Random

        Console.Clear()
        Console.WriteLine("Enemy is firing a missile...")
        Console.WriteLine()
        Threading.Thread.Sleep(1000)

        Do
            validMissileLocation = False
            missileLocationX = randomNum.Next(0, playerBoard.GetLength(0))
            missileLocationY = randomNum.Next(0, playerBoard.GetLength(1))

            If playerBoard(missileLocationX, missileLocationY).IndexOf("X") = -1 Then
                validMissileLocation = True
            End If
        Loop Until validMissileLocation
        enemyMissilesFired += 1
        turnID += 1

        playerBoard(missileLocationX, missileLocationY) = $"{playerBoard(missileLocationX, missileLocationY)}X"
        If playerBoard(missileLocationX, missileLocationY).IndexOf("S") <> -1 Then
            Console.ForegroundColor = ConsoleColor.Red
            Console.Write("HIT!")
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine($" Fired at X: {missileLocationX + 1}, Y: {missileLocationY + 1}")
            enemyMissilesHit += 1
        Else
            Console.ForegroundColor = ConsoleColor.Green
            Console.Write("MISSED!")
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine($" Fired at X: {missileLocationX + 1}, Y: {missileLocationY + 1}")
        End If

        Console.WriteLine()

        Console.WriteLine("YOUR BOARD")
        DisplayBoard(playerBoard, 0)

        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.DarkGray
        Console.WriteLine("Press Enter to continue")
        Console.ForegroundColor = ConsoleColor.White
        Console.ReadLine()
    End Sub

    Sub PlayerTurn()
        Dim validMissileLocation As Boolean
        Dim missileLocationX, missileLocationY As Integer
        Dim choice As String

        Console.WriteLine("ENEMY BOARD")
        DisplayBoard(enemyBoard, 1)
        Console.WriteLine()

        Console.Write("There are ")
        Console.ForegroundColor = ConsoleColor.Red
        Console.Write(ShipsLeft(enemyBoard))
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine(" enemy ship pieces left!")
        Console.WriteLine()

        Do
            validMissileLocation = False

            Try
                Console.Write("Enter a location to fire a missle!")
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine(" (Or type 'save' to save and exit the game)")
                Console.ForegroundColor = ConsoleColor.White
                Console.Write("X: ")

                choice = Console.ReadLine
                If choice.ToLower = "save" Then
                    SaveGame()

                    Console.ForegroundColor = ConsoleColor.Green
                    Console.WriteLine()
                    Console.WriteLine("The game has been saved!")
                    Console.ForegroundColor = ConsoleColor.DarkGray
                    Console.WriteLine()
                    Console.WriteLine("Press Enter to exit")
                    Console.ReadLine()
                    End
                Else
                    missileLocationX = choice - 1
                End If

                Console.Write("Y: ")
                missileLocationY = Console.ReadLine - 1
            Catch ex As Exception

                If ex.GetType.ToString = "System.InvalidCastException" And missileLocationX = 0 Then
                    Console.ForegroundColor = ConsoleColor.DarkGray
                    Console.WriteLine($"Invalid Location! Your X coordinate must be a number between 1 and {enemyBoard.GetLength(0)}")
                    Console.ForegroundColor = ConsoleColor.White
                    Console.WriteLine()
                    Continue Do
                ElseIf ex.GetType.ToString = "System.InvalidCastException" Then
                    Console.ForegroundColor = ConsoleColor.DarkGray
                    Console.WriteLine($"Invalid Location! Your Y coordinate must be a number between 1 and {enemyBoard.GetLength(1)}")
                    Console.ForegroundColor = ConsoleColor.White
                    Console.WriteLine()
                    Continue Do
                Else
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine("An Exception Has Occurred... what did you do?")
                    Console.WriteLine(ex)
                    Console.WriteLine()
                    Console.WriteLine("Press Enter to exit")
                    Console.ReadLine()
                    End
                End If
            End Try

            If missileLocationX < 0 Or missileLocationX > enemyBoard.GetLength(0) - 1 Then
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine($"Invalid Location! Your X coordinate must be a number between 1 and {enemyBoard.GetLength(0)}")
                Console.ForegroundColor = ConsoleColor.White
                Console.WriteLine()
            ElseIf missileLocationY < 0 Or missileLocationY > enemyBoard.GetLength(1) - 1 Then
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine($"Invalid Location! Your Y coordinate must be a number between 1 and {enemyBoard.GetLength(1)}")
                Console.ForegroundColor = ConsoleColor.White
                Console.WriteLine()
            ElseIf enemyBoard(missileLocationX, missileLocationY).IndexOf("X") <> -1 Then
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine($"You've already fired a missile at that location! Please choose another.")
                Console.ForegroundColor = ConsoleColor.White
                Console.WriteLine()
            Else
                validMissileLocation = True
            End If
        Loop Until validMissileLocation

        Console.Clear()
        Console.WriteLine("Firing Missile...")
        Console.WriteLine()
        playerMissilesFired += 1
        turnID += 1
        Threading.Thread.Sleep(1000)

        enemyBoard(missileLocationX, missileLocationY) = $"{enemyBoard(missileLocationX, missileLocationY)}X"
        If enemyBoard(missileLocationX, missileLocationY).IndexOf("S") <> -1 Then
            Console.ForegroundColor = ConsoleColor.Green
            Console.Write("HIT!")
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine($" Fired at X: {missileLocationX + 1}, Y: {missileLocationY + 1}")
            playerMissilesHit += 1
        Else
            Console.ForegroundColor = ConsoleColor.Red
            Console.Write("MISSED!")
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine($" Fired at X: {missileLocationX + 1}, Y: {missileLocationY + 1}")
        End If
        Console.WriteLine()

        Console.WriteLine("ENEMY BOARD")
        DisplayBoard(enemyBoard, 1)

        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.DarkGray
        Console.WriteLine("Press Enter to continue")
        Console.ForegroundColor = ConsoleColor.White
        Console.ReadLine()
    End Sub

    Sub DisplayBoard(gameBoard, displayMode)
        If displayMode = 1 Then
            For y = -1 To gameBoard.GetLength(0) - 1
                For x = -1 To gameBoard.GetLength(1) - 1
                    If x = -1 And y = -1 Then
                        Console.ForegroundColor = ConsoleColor.DarkGray
                        Console.Write("# ")
                        Console.ForegroundColor = ConsoleColor.White
                    ElseIf y = -1 Then
                        Console.ForegroundColor = ConsoleColor.DarkGray
                        Console.Write($"{x + 1} ")
                        Console.ForegroundColor = ConsoleColor.White
                    ElseIf x = -1 Then
                        Console.ForegroundColor = ConsoleColor.DarkGray
                        Console.Write($"{y + 1} ")
                        Console.ForegroundColor = ConsoleColor.White
                    Else
                        If gameBoard(x, y).IndexOf("S") <> -1 And gameBoard(x, y).IndexOf("X") <> -1 Then
                            Console.ForegroundColor = ConsoleColor.Green
                        ElseIf gameBoard(x, y).IndexOf("X") <> -1 Then
                            Console.ForegroundColor = ConsoleColor.Red
                        Else
                            Console.ForegroundColor = ConsoleColor.Blue
                        End If
                        Console.Write($"~ ")
                    End If
                Next
                Console.WriteLine()
            Next
        ElseIf displayMode = 2 Then
            For y = -1 To gameBoard.GetLength(0) - 1
                For x = -1 To gameBoard.GetLength(1) - 1
                    If x = -1 And y = -1 Then
                        Console.ForegroundColor = ConsoleColor.DarkGray
                        Console.Write("# ")
                        Console.ForegroundColor = ConsoleColor.White
                    ElseIf y = -1 Then
                        Console.ForegroundColor = ConsoleColor.DarkGray
                        Console.Write($"{x + 1} ")
                        Console.ForegroundColor = ConsoleColor.White
                    ElseIf x = -1 Then
                        Console.ForegroundColor = ConsoleColor.DarkGray
                        Console.Write($"{y + 1} ")
                        Console.ForegroundColor = ConsoleColor.White
                    Else
                        If gameBoard(x, y).IndexOf("X") <> -1 Then
                            Console.ForegroundColor = ConsoleColor.Red
                        ElseIf gameBoard(x, y).IndexOf("S") <> -1 Then
                            Console.ForegroundColor = ConsoleColor.DarkYellow
                        Else
                            Console.ForegroundColor = ConsoleColor.Blue
                        End If
                        Console.Write($"{gameBoard(x, y)} ")
                    End If
                Next
                Console.WriteLine()
            Next
        Else
            For y = -1 To gameBoard.GetLength(0) - 1
                For x = -1 To gameBoard.GetLength(1) - 1
                    If x = -1 And y = -1 Then
                        Console.ForegroundColor = ConsoleColor.DarkGray
                        Console.Write("# ")
                        Console.ForegroundColor = ConsoleColor.White
                    ElseIf y = -1 Then
                        Console.ForegroundColor = ConsoleColor.DarkGray
                        Console.Write($"{x + 1} ")
                        Console.ForegroundColor = ConsoleColor.White
                    ElseIf x = -1 Then
                        Console.ForegroundColor = ConsoleColor.DarkGray
                        Console.Write($"{y + 1} ")
                        Console.ForegroundColor = ConsoleColor.White
                    Else
                        If gameBoard(x, y).IndexOf("S") <> -1 And gameBoard(x, y).IndexOf("X") <> -1 Then
                            Console.ForegroundColor = ConsoleColor.Red
                            Console.Write($"{gameBoard(x, y)(0)} ")
                            Continue For
                        ElseIf gameBoard(x, y).IndexOf("X") <> -1 Then
                            Console.ForegroundColor = ConsoleColor.Green
                        ElseIf gameBoard(x, y).IndexOf("S") <> -1 Then
                            Console.ForegroundColor = ConsoleColor.DarkYellow
                            Console.Write($"{gameBoard(x, y)(0)} ")
                            Continue For
                        Else
                            Console.ForegroundColor = ConsoleColor.Blue
                        End If
                        Console.Write($"~ ")
                    End If
                Next
                Console.WriteLine()
            Next
        End If

        Console.ForegroundColor = ConsoleColor.White
    End Sub

    Sub GenerateBoard(ByRef gameBoard)
        Dim availableShips As Integer() = {5, 4, 3, 3, 2, 2}
        Dim randomNum As New Random
        Dim shipX, shipY, tempX, tempY, shipDirection, shipSize As Integer
        Dim validShipPlacement As Boolean

        For y = 0 To gameBoard.GetLength(0) - 1
            For x = 0 To gameBoard.GetLength(1) - 1
                gameBoard(x, y) = "O"
            Next
        Next

        For shipNum = 1 To availableShips.Length
            Do
                Randomize()
                shipSize = randomNum.Next(2, 6)

                If availableShips.Contains(shipSize) Then
                    For i = 0 To availableShips.Length - 1
                        If availableShips(i) = shipSize Then
                            availableShips(i) = -1
                            Exit Do
                        End If
                    Next
                    Exit Do
                End If
            Loop

            Do
                validShipPlacement = False
                Randomize()
                shipX = randomNum.Next(0, gameBoard.GetLength(0))
                tempX = shipX
                Randomize()
                shipY = randomNum.Next(0, gameBoard.GetLength(1))
                tempY = shipY
                Randomize()
                shipDirection = randomNum.Next(0, 4)

                If shipDirection = 0 Then
                    For i = 0 To shipSize - 1
                        tempY = shipY - i

                        If tempY < 0 Then
                            validShipPlacement = False
                            Exit For
                        ElseIf gameBoard(tempX, tempY) <> "O" Then
                            validShipPlacement = False
                            Exit For
                        End If

                        validShipPlacement = True
                    Next
                ElseIf shipDirection = 1 Then
                    For i = 0 To shipSize - 1
                        tempX = shipX + i

                        If tempX > 7 Then
                            validShipPlacement = False
                            Exit For
                        ElseIf gameBoard(tempX, tempY) <> "O" Then
                            validShipPlacement = False
                            Exit For
                        End If

                        validShipPlacement = True
                    Next
                ElseIf shipDirection = 2 Then
                    For i = 0 To shipSize - 1
                        tempY = shipY + i

                        If tempY > 7 Then
                            validShipPlacement = False
                            Exit For
                        ElseIf gameBoard(tempX, tempY) <> "O" Then
                            validShipPlacement = False
                            Exit For
                        End If

                        validShipPlacement = True
                    Next
                ElseIf shipDirection = 3 Then
                    For i = 0 To shipSize - 1
                        tempX = shipX - i

                        If tempX < 0 Then
                            validShipPlacement = False
                            Exit For
                        ElseIf gameBoard(tempX, tempY) <> "O" Then
                            validShipPlacement = False
                            Exit For
                        End If

                        validShipPlacement = True
                    Next
                End If

                If validShipPlacement Then
                    tempX = shipX
                    tempY = shipY

                    If shipDirection = 0 Then
                        For i = 1 To shipSize
                            gameBoard(tempX, tempY) = $"{shipSize}SV"
                            tempY -= 1
                        Next
                    ElseIf shipDirection = 1 Then
                        For i = 1 To shipSize
                            gameBoard(tempX, tempY) = $"{shipSize}SH"
                            tempX += 1
                        Next
                    ElseIf shipDirection = 2 Then
                        For i = 1 To shipSize
                            gameBoard(tempX, tempY) = $"{shipSize}SV"
                            tempY += 1
                        Next
                    ElseIf shipDirection = 3 Then
                        For i = 1 To shipSize
                            gameBoard(tempX, tempY) = $"{shipSize}SH"
                            tempX -= 1
                        Next
                    End If
                End If
                Console.WriteLine()
            Loop Until validShipPlacement = True
        Next
    End Sub
End Module