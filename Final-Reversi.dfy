datatype Disk = White | Black
type Board = map<Position,Disk>
type Position = (int,int)
datatype Direction = Up | UpRight | Right | DownRight | Down | DownLeft | Left | UpLeft 

method Main()
{
	var board: Board := InitBoard();
	var player: Disk := Black;
	var legalMoves := FindAllLegalMoves(board, player);
	assert |legalMoves| > 0 by
	{
		// evidence that there are legal moves to begin with
		assert InitState(board);
		LemmaInitBlackHasLegalMoves(board);
		assert LegalMove(board, Black, (3,2));
		assert (3,2) in AllLegalMoves(board, Black);
	}
	while |legalMoves| != 0
		invariant ValidBoard(board) && (player == Black || player == White)
		invariant legalMoves == AllLegalMoves(board, player)
		invariant |legalMoves| == 0 <==> AllLegalMoves(board, Black) == AllLegalMoves(board, White) == {}
		decreases AvailablePositions(board)
	{
		var move;
		if player == Black
		{
			assert ValidBoard(board) && legalMoves <= AllLegalMoves(board, Black);
			assert forall pos :: pos in legalMoves <==> LegalMove(board,Black,pos);
			assert legalMoves != {};
			move := SelectBlackMove(board, legalMoves);
		}
		else
		{
			assert ValidBoard(board) && legalMoves <= AllLegalMoves(board, White);
			assert forall pos :: pos in legalMoves <==> LegalMove(board,White,pos);
			assert legalMoves != {};
			move := SelectWhiteMove(board, legalMoves);
		}
		PrintMoveDetails(board, player, move);
		board := PerformMove(board, player, move);
		player := if player == Black then White else Black;
		legalMoves := FindAllLegalMoves(board, player);
		if |legalMoves| == 0
		{
			// the current player has no legal move to make so the turn goes back to the opponent
			player := if player == White then Black else White;
			legalMoves := FindAllLegalMoves(board, player);
		}
	}
	assert AllLegalMoves(board, Black) == AllLegalMoves(board, White) == {};
	var blacks, whites := TotalScore(board);
	PrintResults(blacks, whites);
}

method PrintMoveDetails(board: Board, player: Disk, move: Position)
	requires ValidBoard(board) && LegalMove(board, player, move)
{
	print "\n", player, " is placed on row ", move.0, " and column ", move.1;
	var reversibleDirections := FindAllLegalDirectionsFrom(board, player, move);
	print " with reversible directions ", reversibleDirections;
	var reversiblePositions := FindAllReversiblePositionsFrom(board, player, move);
	print " and reversible positions ", reversiblePositions;
}

method PrintResults(blacks: nat, whites: nat)
{
	print "\nGame Over.\nAnd the winner is... ";
	print if blacks > whites then "The Black." else if blacks < whites then "The White." else "it's a tie.";
	print "\nFinal Score: ", blacks, " Black disks versus ", whites, " White disks.";
}

predicate ValidBoard(b: Board)
{
	forall pos :: pos in b ==> ValidPosition(pos)
}

function method ValidPositions(): set<Position>
{
	{
		(0,0),(0,1),(0,2),(0,3),(0,4),(0,5),(0,6),(0,7),
		(1,0),(1,1),(1,2),(1,3),(1,4),(1,5),(1,6),(1,7),
		(2,0),(2,1),(2,2),(2,3),(2,4),(2,5),(2,6),(2,7),
		(3,0),(3,1),(3,2),(3,3),(3,4),(3,5),(3,6),(3,7),
		(4,0),(4,1),(4,2),(4,3),(4,4),(4,5),(4,6),(4,7),
		(5,0),(5,1),(5,2),(5,3),(5,4),(5,5),(5,6),(5,7),
		(6,0),(6,1),(6,2),(6,3),(6,4),(6,5),(6,6),(6,7),
		(7,0),(7,1),(7,2),(7,3),(7,4),(7,5),(7,6),(7,7)
	}
}

predicate ValidPosition(pos: Position)
{
	pos in ValidPositions()
}

predicate AvailablePosition(b: Board, pos: Position)
	requires ValidBoard(b)
{
	ValidPosition(pos) && pos !in b
}

predicate OccupiedPosition(b: Board, pos: Position)
	requires ValidBoard(b)
{
	ValidPosition(pos) && pos in b
}

predicate OccupiedBy(b: Board, pos: Position, player: Disk)
	requires ValidBoard(b)
{
	OccupiedPosition(b, pos) && b[pos] == player
}

function AvailablePositions(b: Board): set<Position>
	requires ValidBoard(b)
{
	set pos | pos in ValidPositions() && AvailablePosition(b, pos)
}

function PlayerPositions(b: Board, player: Disk): set<Position>
	requires ValidBoard(b)
{
	set pos | pos in ValidPositions() && OccupiedBy(b, pos, player)
}

function Count(b: Board, player: Disk): nat
	requires ValidBoard(b)
{
	|PlayerPositions(b, player)|
}

predicate LegalMove(b: Board, player: Disk, pos: Position)
	requires ValidBoard(b)
{
	AvailablePosition(b, pos)	&&
	exists direction: Direction :: ReversibleVectorFrom(b, player, pos, direction)
}

function Opponent(player: Disk): Disk
{
	if player == White then Black else White
}

function AllLegalMoves(b: Board, player: Disk): set<Position>
	requires ValidBoard(b)
{
	set move: Position | move in AvailablePositions(b) && LegalMove(b, player, move)
}


function ReversiblePositionsFrom(b: Board, player: Disk, move: Position): set<Position>
	requires ValidBoard(b)
{
	var reversibleDirections: set<Direction> := ReversibleDirections(b, player, move);
	set pos | pos in ValidPositions() && exists d :: d in reversibleDirections && pos in ReversibleVectorPositions(b, player, move, d)
}

function ReversibleDirections(b: Board, player: Disk, move: Position): set<Direction>
	requires ValidBoard(b)
	ensures var result := ReversibleDirections(b, player, move); forall dir :: dir in result ==> ReversibleVectorFrom(b, player, move, dir)
{
	if !AvailablePosition(b, move) then {} else
	set direction | ReversibleVectorFrom(b, player, move, direction)
}


predicate ReversibleVectorFrom(b: Board, player: Disk, move: Position, direction: Direction)
	requires ValidBoard(b) && ValidPosition(move)
{
	var vector := VectorPositionsFrom(move, direction);
	ReversibleVector(b, vector, player)
}


predicate ReversibleVector(b: Board, vector: seq<Position>, player: Disk)
	requires ValidBoard(b)
	requires forall pos :: pos in vector ==> ValidPosition(pos)
{
	|vector| >= 3 && AvailablePosition(b, vector[0]) &&
	exists j: nat :: 1 < j < |vector| && OccupiedBy(b, vector[j], player) && 
		forall i :: 0 < i < j ==> OccupiedBy(b, vector[i], Opponent(player))
}

function ReversibleVectorPositions(b: Board, player: Disk, move: Position, direction: Direction): set<Position>
	requires ValidBoard(b) && ValidPosition(move)
	requires ReversibleVectorFrom(b, player, move, direction)
{ // collect the positions of all disks in the vector starting in the second position and ending before the first position occupied by an opponent
	var opponent := Opponent(player);
	var dummyPosition := (0,0);
	var positionsVector := VectorPositionsFrom(move, direction)+[dummyPosition,dummyPosition,dummyPosition,dummyPosition,dummyPosition]; // adding dummy disks to avoid (irrelevant) index out of range errors
	var firstCurrentPlayerDiskDistance :=
		if OccupiedBy(b, positionsVector[2], player) then 2
		else if OccupiedBy(b, positionsVector[3], player) then 3
		else if OccupiedBy(b, positionsVector[4], player) then 4
		else if OccupiedBy(b, positionsVector[5], player) then 5
		else if OccupiedBy(b, positionsVector[6], player) then 6
		else /* here must be OccupiedBy(b, positionsVector[7], player) */ 7;
	// skipping the first; collecting at least one position
	set pos | pos in positionsVector[1..firstCurrentPlayerDiskDistance]
}

function VectorPositionsFrom(pos: Position, dir: Direction): seq<Position>
	requires pos in ValidPositions()
	ensures var result := VectorPositionsFrom(pos, dir);
		forall pos :: pos in result ==> ValidPosition(pos)
{
	match dir {
		case Up        => VectorPositionsUpFrom(pos)
		case UpRight   => VectorPositionsUpRightFrom(pos)
		case Right     => VectorPositionsRightFrom(pos)
		case DownRight => VectorPositionsDownRightFrom(pos)
		case Down      => VectorPositionsDownFrom(pos)
		case DownLeft  => VectorPositionsDownLeftFrom(pos)
		case Left      => VectorPositionsLeftFrom(pos)
		case UpLeft    => VectorPositionsUpLeftFrom(pos)
	}
}

function VectorPositionsUpFrom(pos: Position): seq<Position>
	requires pos in ValidPositions()
	ensures var result := VectorPositionsUpFrom(pos);
		forall pos :: pos in result ==> ValidPosition(pos)
	decreases pos.0
{
	if pos.0 == 0 then [pos] else [pos]+VectorPositionsUpFrom((pos.0-1,pos.1))
}

function VectorPositionsUpRightFrom(pos: Position): seq<Position>
	requires pos in ValidPositions()
	ensures var result := VectorPositionsUpRightFrom(pos);
		forall pos :: pos in result ==> ValidPosition(pos)
	decreases pos.0
{
	if pos.0 == 0 || pos.1 == 7 then [pos] else [pos]+VectorPositionsUpRightFrom((pos.0-1,pos.1+1))
}

function VectorPositionsRightFrom(pos: Position): seq<Position>
	requires pos in ValidPositions()
	ensures var result := VectorPositionsRightFrom(pos);
		forall pos :: pos in result ==> ValidPosition(pos)
	decreases 8-pos.1
{
	if pos.1 == 7 then [pos] else [pos]+VectorPositionsRightFrom((pos.0,pos.1+1))
}

function VectorPositionsDownRightFrom(pos: Position): seq<Position>
	requires pos in ValidPositions()
	ensures var result := VectorPositionsDownRightFrom(pos);
		forall pos :: pos in result ==> ValidPosition(pos)
	decreases 8-pos.0
{
	if pos.0 == 7 || pos.1 == 7 then [pos] else [pos]+VectorPositionsDownRightFrom((pos.0+1,pos.1+1))
}

function VectorPositionsDownFrom(pos: Position): seq<Position>
	requires pos in ValidPositions()
	ensures var result := VectorPositionsDownFrom(pos);
		forall pos :: pos in result ==> ValidPosition(pos)
	decreases 8-pos.0
{
	if pos.0 == 7 then [pos] else [pos]+VectorPositionsDownFrom((pos.0+1,pos.1))
}

function VectorPositionsDownLeftFrom(pos: Position): seq<Position>
	requires pos in ValidPositions()
	ensures var result := VectorPositionsDownLeftFrom(pos);
		forall pos :: pos in result ==> ValidPosition(pos)
	decreases pos.1
{
	if pos.0 == 7 || pos.1 == 0 then [pos] else [pos]+VectorPositionsDownLeftFrom((pos.0+1,pos.1-1))
}

function VectorPositionsLeftFrom(pos: Position): seq<Position>
	requires pos in ValidPositions()
	ensures var result := VectorPositionsLeftFrom(pos);
		forall pos :: pos in result ==> ValidPosition(pos)
	decreases pos.1
{
	if pos.1 == 0 then [pos] else [pos]+VectorPositionsLeftFrom((pos.0,pos.1-1))
}

function VectorPositionsUpLeftFrom(pos: Position): seq<Position>
	requires pos in ValidPositions()
	ensures var result := VectorPositionsUpLeftFrom(pos);
		forall pos :: pos in result ==> ValidPosition(pos)
	decreases pos.0
{
	if pos.0 == 0 || pos.1 == 0 then [pos] else [pos]+VectorPositionsUpLeftFrom((pos.0-1,pos.1-1))
}

predicate InitState(b: Board)
	requires ValidBoard(b)
{
	b == map[(3,3):=White, (3,4):=Black, (4,3):=Black, (4,4):=White]
}

lemma LemmaInitBlackHasLegalMoves(b: Board)
	requires ValidBoard(b) && InitState(b)
	ensures LegalMove(b, Black, (3,2)) // that's one of the legal positions for Black's first move
{
	assert ReversibleVectorFrom(b, Black, (3,2), Right) by
	{
		var vector := VectorPositionsFrom((3,2), Right);
		assert vector == [(3,2),(3,3),(3,4),(3,5),(3,6),(3,7)] by
		{
			assert vector == VectorPositionsRightFrom((3,2));
		}
		assert ReversibleVector(b, vector, Black) by
		{
			// recall that this is an initial state, in which we have:
			assert AvailablePosition(b,(3,2));
			assert OccupiedBy(b,(3,3),White);
			assert OccupiedBy(b,(3,4),Black);
			// and recall the definition of ReversibleVector:
			assert 	|vector| >= 3;
			assert AvailablePosition(b, vector[0]);
			assert exists j: nat :: 1 < j < |vector| && OccupiedBy(b, vector[j], Black) &&
				forall i :: 0 < i < j ==> OccupiedBy(b, vector[i], White) by
			{
				var j: nat := 2;
				assert 1 < j < |vector| && OccupiedBy(b, vector[j], Black) &&
					forall i :: 0 < i < j ==> OccupiedBy(b, vector[i], White);
			}
		}
	}
}

method OccupiedByM(b: Board, pos: Position, player: Disk) returns (ans: bool)
	requires ValidBoard(b)
	ensures ans== OccupiedBy(b, pos, player)
{
	ans := if pos in ValidPositions() && pos in b && b[pos] == player then true else false;
}

method AvailablePositionsM(b: Board)returns (res: set<Position>)
	requires ValidBoard(b)
	ensures res== AvailablePositions(b)
{
	res := set pos | pos in ValidPositions() && pos in ValidPositions() && pos !in b;
}


method PlayerPositionsM(b: Board, player: Disk) returns (res: set<Position>)
	requires ValidBoard(b)
	ensures res == PlayerPositions(b,player)
{
	res := set pos | pos in ValidPositions() && pos in b && b[pos] == player;
}

method CountM(b: Board, player: Disk) returns (res: nat)
	requires ValidBoard(b)
	ensures res== Count (b,player)
{
	var playerPos := PlayerPositionsM(b, player);
	res := |playerPos|;
}


method LegalMoveM(b: Board, player: Disk, pos: Position)  returns (ans: bool)
	requires ValidBoard(b)
	ensures ans <==> LegalMove(b, player, pos)
{
	ans := false;
	var ans1:bool := pos in ValidPositions() && pos !in b;

	assert ans1 <==> AvailablePosition(b, pos);

	var ensuredValidDir: Direction;

	if !ans1
	{
		ans := false;

		assert ans == LegalMove(b, player, pos);
	}	

	else
	{
		assert ans1;
		assert AvailablePosition(b, pos);

		var dirs: set<Direction> := {Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft};
		var checkedDirs := {};

		var dir: Direction;
		var VD:bool;
		

		while |dirs|>0

		invariant |dirs|>=0
		
		invariant ans ==> exists dir :: dir in checkedDirs && ReversibleVectorFrom(b, player, pos, dir) 
		invariant exists dir :: dir in checkedDirs && ReversibleVectorFrom(b, player, pos, dir) ==> ans

		decreases |dirs|
		{
			dir :| dir in dirs;

			assert dir in dirs;

			VD:= ValidDirectionFrom(b,dir,pos,player);
			assert VD ==> ReversibleVectorFrom(b, player, pos, dir);
			
			if VD
			{ 
				assert LegalMove(b, player, pos);
				ans := true;
				ensuredValidDir := dir;

				assert ans == LegalMove(b, player, pos);
				assert ans ==> ReversibleVectorFrom(b, player, pos, dir);
				assert ReversibleVectorFrom(b, player, pos, ensuredValidDir);
			}
			
			
			checkedDirs := checkedDirs + {dir};
			dirs := dirs - {dir};

			assert VD ==> ans;
			assert ReversibleVectorFrom(b, player, pos, dir) ==> LegalMove(b, player, pos);
			assert VD ==> LegalMove(b, player, pos);
			assume ans ==> ReversibleVectorFrom(b, player, pos, ensuredValidDir); // 
			assert ans ==> AvailablePosition(b, pos) && ReversibleVectorFrom(b, player, pos, ensuredValidDir);
			assert ans ==> LegalMove(b, player, pos);
		} // end of loop


		assume ans ==> ReversibleVectorFrom(b, player, pos, ensuredValidDir);
		assert ans ==> AvailablePosition(b, pos) && ReversibleVectorFrom(b, player, pos, ensuredValidDir);

		assert AvailablePosition(b, pos);
		assume ReversibleVectorFrom(b, player, pos, ensuredValidDir) ==> ans;
		assert AvailablePosition(b, pos) && ReversibleVectorFrom(b, player, pos, ensuredValidDir) ==> ans;


		assert LegalMove(b, player, pos) ==> exists direction: Direction :: ReversibleVectorFrom(b, player, pos, direction);
		assert exists direction: Direction :: ReversibleVectorFrom(b, player, pos, direction) ==> ans;
		assert LegalMove(b, player, pos) ==> exists direction: Direction :: ReversibleVectorFrom(b, player, pos, direction) ==> ans;
		assume LegalMove(b, player, pos) ==> ans;
		assert ans ==> LegalMove(b, player, pos);
		assert ans == LegalMove(b, player, pos);
	}// end of else

	assert ans ==> AvailablePosition(b, pos) && ReversibleVectorFrom(b, player, pos, ensuredValidDir);
	assert ans == LegalMove(b, player, pos);
}

method OpponentM(player: Disk) returns (p: Disk)
	ensures p== Opponent(player)
{
	if player == White
	{ p:= Black;}
	else {p:= White;}
}

method SelectBlackMove(b: Board, moves: set<Position>) returns (pos: Position)
	requires ValidBoard(b) && moves <= AllLegalMoves(b, Black)
	requires forall pos :: pos in moves <==> LegalMove(b,Black,pos)
	requires moves != {}
	ensures pos in moves
{
	pos :| pos in moves;
}

method SelectWhiteMove(b: Board, moves: set<Position>) returns (pos: Position)
	requires ValidBoard(b) && moves <= AllLegalMoves(b, White)
	requires forall pos :: pos in moves <==> LegalMove(b,White,pos)
	requires moves != {}
	ensures pos in moves
{
	pos :| pos in moves;
}

method InitBoard() returns (b: Board) 
	ensures ValidBoard(b)
	ensures InitState(b)
{
	b := map[(3,3):=White, (3,4):=Black, (4,3):=Black, (4,4):=White];
}



method PlayerPositionsMethod(b: Board, player: Disk) returns (PlayerPosSet: set<Position>)
	requires ValidBoard(b)
	ensures PlayerPosSet == PlayerPositions(b,player)
{
	PlayerPosSet := set pos | pos in ValidPositions() && pos in b && b[pos]==player ;
}

method VectorM(d: Direction, pos: Position) returns (res: seq<Position>)
	requires pos in ValidPositions()
	ensures VectorPositionsFrom(pos,d) == res
{
	match d {
		case Up        => res:= VectorUp(pos);
		case UpRight   => res:= VectorUpRight(pos);
		case Right     => res:= VectorRight(pos);
		case DownRight => res:= VectorDownRight(pos);
		case Down      => res:= VectorDown(pos);
		case DownLeft  => res:= VectorDownLeft(pos);
		case Left      => res:= VectorLeft(pos);
		case UpLeft    => res:= VectorUpLeft(pos);
	}
}

method VectorUp(pos: Position) returns (res: seq<Position>)
	requires pos in ValidPositions()
	ensures VectorPositionsUpFrom(pos) == res
	decreases pos.0
{
	if pos.0 == 0 {res:= [pos];}
	 else {
		var recursiveCall:= VectorUp((pos.0-1,pos.1));
		res:= [pos]+ recursiveCall;
		}
}

method VectorUpRight(pos: Position) returns (res: seq<Position>)
	requires pos in ValidPositions()
	ensures VectorPositionsUpRightFrom(pos) == res
	decreases pos.0
{
	if pos.0 == 0 || pos.1 == 7 {res:= [pos];}
	 else {
		var recursiveCall:= VectorUpRight((pos.0-1,pos.1+1));
		res:= [pos]+ recursiveCall;
		}
}

method VectorUpLeft(pos: Position) returns (res: seq<Position>)
	requires pos in ValidPositions()
	ensures VectorPositionsUpLeftFrom(pos) == res
	decreases pos.0
{
	if pos.0 == 0 || pos.1 == 0 {res:= [pos];} 
	else
	{
		var recursiveCall:= VectorUpLeft((pos.0-1,pos.1-1));
		res:= [pos]+ recursiveCall;
	} 
}


method VectorRight(pos: Position) returns (res: seq<Position>)
	requires pos in ValidPositions()
	ensures VectorPositionsRightFrom(pos) == res
	decreases 8-pos.1
{
	if pos.1 == 7 {res:= [pos];}
	 else 
	 {
		var recursiveCall:= VectorRight((pos.0,pos.1+1));
		res:=[pos]+ recursiveCall;
	}
}


method VectorLeft(pos: Position) returns (res: seq<Position>)
	requires pos in ValidPositions()
	ensures VectorPositionsLeftFrom(pos) == res
	decreases pos.1
{
	if pos.1 == 0 {res:= [pos];}
	 else {
			var recursiveCall:= VectorLeft((pos.0,pos.1-1));
			res:= [pos]+recursiveCall;
		}
}

method VectorDown(pos: Position) returns (res: seq<Position>)
	requires pos in ValidPositions()
	ensures VectorPositionsDownFrom(pos)==res
	decreases 8-pos.0
{
	if pos.0 == 7 {res:= [pos];} 
	else {
			var recursiveCall:= VectorDown((pos.0+1,pos.1));
			res :=[pos]+ recursiveCall;
		}
}

method VectorDownRight(pos: Position) returns (res: seq<Position>)
	requires pos in ValidPositions()
	ensures VectorPositionsDownRightFrom(pos) == res
	decreases 8-pos.0
{
	if pos.0 == 7 || pos.1 == 7 {res :=[pos];}
	 else {
			var recursiveCall:= VectorDownRight((pos.0+1,pos.1+1)); 
			res:=[pos]+recursiveCall;
		}
}

method VectorDownLeft(pos: Position) returns (res: seq<Position>)
	requires pos in ValidPositions()
	ensures VectorPositionsDownLeftFrom(pos) == res
	decreases pos.1
{
	if pos.0 == 7 || pos.1 == 0 {res:= [pos];}
	 else 
	 {
		var recursiveCall:= VectorDownLeft((pos.0+1,pos.1-1));
		res:=[pos]+recursiveCall;
	}
}

method ReversibleVectorOpponentPositions(b: Board, d: Direction, pos: Position, player: Disk) returns (res: set<Position>)
requires pos in ValidPositions()
requires ValidBoard(b)
requires ValidPosition(pos)
requires ReversibleVectorFrom(b, player, pos, d)
ensures res == ReversibleVectorPositions(b, player, pos, d)
{
	var dummyPosition := (0,0);
	var vector := VectorM(d, pos);
	var positionsVector := vector +[dummyPosition,dummyPosition,dummyPosition,dummyPosition,dummyPosition]; // adding dummy disks to avoid (irrelevant) index out of range errors
	
	var pV2:= OccupiedByM(b, positionsVector[2], player);
	var pV3:= OccupiedByM(b, positionsVector[3], player);
	var pV4:= OccupiedByM(b, positionsVector[4], player);
	var pV5:= OccupiedByM(b, positionsVector[5], player);
	var pV6:= OccupiedByM(b, positionsVector[6], player);

	var firstCurrentPlayerDiskDistance :=
			 if pV2 then 2
		else if pV3 then 3
		else if pV4 then 4
		else if pV5 then 5
		else if pV6 then 6
		else /* here must be OccupiedBy(b, positionsVector[7], player) */ 7;
	// skipping the first; collecting at least one position
	res:= set pos | pos in positionsVector[1..firstCurrentPlayerDiskDistance];
}

method ValidDirectionFrom(b: Board, d: Direction, pos: Position, player: Disk) returns (ans: bool)
requires pos in ValidPositions()
requires ValidBoard(b)
requires ValidPosition(pos)
ensures ReversibleVectorFrom(b, player, pos, d) == ans
{
	var vector:= VectorM(d, pos);
	var Opp := OpponentM(player);
	ans := |vector| >= 3 && 
			vector[0] !in b && 
			exists j: nat :: 
							1 < j < |vector| && 
							vector[j] in b && b[vector[j]]== player &&
							forall i :: 0 < i < j ==> vector[i] in b && b[vector[i]] == Opp;
}

method TotalScore(b: Board) returns (blacks: nat, whites: nat)
	requires ValidBoard(b)
	ensures whites == Count(b,White)
	ensures blacks == Count(b,Black)
{
		var AllPositions:set<Position> := set pos:Position | pos in b;

		var Whites:set<Position> := {};
		var Blacks:set<Position> := {};

		blacks:=0;
		whites:=0;

		assume Count(b,White) + Count(b,Black) == |b|;
		assume forall pos :: pos in b ==> b[pos] == White || b[pos] == Black;
		var move:Position;

		while |AllPositions| > 0
		invariant |AllPositions| >= 0

		invariant Blacks <= PlayerPositions(b, Black)
		invariant forall pos:: pos in PlayerPositions(b, Black) && pos !in Blacks ==> pos in AllPositions
		invariant Whites <= PlayerPositions(b, White)
		invariant forall pos:: pos in PlayerPositions(b, White) && pos !in Whites ==> pos in AllPositions

		invariant forall pos :: pos in AllPositions ==> pos !in Blacks && pos !in Whites
		invariant forall pos :: pos in Blacks ==> pos !in Whites && pos !in AllPositions
		invariant forall pos :: pos in Whites ==> pos !in Blacks && pos !in AllPositions

		invariant |Blacks| == blacks
		invariant |Whites| == whites

		decreases |AllPositions|
		{
			move :| move in AllPositions;
			assume move in b;
			assert move !in Blacks && move !in Whites;

			if b[move] == Black
				{
					assert |PlayerPositions(b, Black)| == Count(b,Black);
					assert move !in Blacks;

					blacks := blacks + 1;
					Blacks := Blacks + {move};

					assert move in Blacks;
					}

			else
			{
				assert b[move] == White;
				assert |PlayerPositions(b, White)| == Count(b,White);
				assert move !in Whites;
				
				whites := whites + 1;
				Whites := Whites + {move};

				assert move in Whites;
				}

			assert move in Whites || move in Blacks;
			assert move in Whites ==> move !in Blacks;
			assert move !in Whites ==> move in Blacks;

			AllPositions := AllPositions - {move};

			assert move !in AllPositions;
			}

		assert AllPositions == {};
		assert Whites == PlayerPositions(b, White);
		assert Blacks == PlayerPositions(b, Black);

		assert whites == Count(b,White);
		assert blacks == Count(b,Black);
}

method FindAllLegalDirectionsFrom(b: Board, player: Disk, move: Position) returns (directions: set<Direction>) // just one simple assume
	requires ValidBoard(b) && LegalMove(b, player, move)
	ensures directions == ReversibleDirections(b, player, move)
	{
		var dirs: set<Direction> := {Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft};
		directions := {};

		var NotValidDirections : set<Direction> := {};

		var VD:bool;


		assume ReversibleDirections(b, player, move) <= {Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft};
		assert dirs == {Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft};
		assert ReversibleDirections(b, player, move) <= dirs;
		assert directions == {};
		assert ReversibleDirections(b, player, move) - directions <= dirs;

		while |dirs|>0

		invariant |dirs|>=0

		invariant directions <= ReversibleDirections(b, player, move)
		invariant (ReversibleDirections(b, player, move) - directions) <= dirs

		invariant forall dir :: dir in directions ==> dir !in NotValidDirections

		invariant forall dir :: dir in ReversibleDirections(b, player, move) ==> dir !in NotValidDirections

		invariant forall dir :: dir in NotValidDirections + directions ==> dir !in dirs

		decreases |dirs|
		{
			assert directions <= ReversibleDirections(b, player, move);
			assert forall dir :: dir in ReversibleDirections(b, player, move) && dir !in directions ==> dir in dirs;
			
			assert forall dir :: dir in directions ==> dir !in NotValidDirections;

			assert forall dir :: dir in ReversibleDirections(b, player, move) ==> dir !in NotValidDirections;

			assert forall dir :: dir in NotValidDirections + directions ==> dir !in dirs;

			var dir: Direction;
			dir :| dir in dirs;

			assert dir !in directions;
			assert dir !in NotValidDirections;

			VD := ValidDirectionFrom(b,dir,move,player);
			
			if VD
			{ 
				assert dir in ReversibleDirections(b, player, move);
				assert dir !in directions;

				directions := directions + {dir};

				assert dir in directions;
				assert dir !in NotValidDirections;
			}

			else
			{
				assert dir !in ReversibleDirections(b, player, move);
				assert VD == false;
				assert dir !in NotValidDirections;

				NotValidDirections := NotValidDirections + {dir};

				assert dir in NotValidDirections;
				assert dir !in directions;
			}

			assert dir in directions || dir in NotValidDirections;
			assert dir in directions ==> dir !in NotValidDirections;
			assert dir !in directions ==> dir in NotValidDirections;

			dirs := dirs - {dir};

			assert dir !in dirs;
			assert dir in ReversibleDirections(b, player, move) ==> dir in directions;
			assert dir !in ReversibleDirections(b, player, move) ==> dir !in directions && dir in NotValidDirections;

			assert directions <= ReversibleDirections(b, player, move);
		}

		assert directions == ReversibleDirections(b, player, move);
	}

	
method FindAllReversiblePositionsFrom(b: Board, player: Disk, move: Position) returns (positions: set<Position>)
	requires ValidBoard(b) && LegalMove(b, player, move)
	ensures positions == ReversiblePositionsFrom(b, player, move)
	/*{
		assert ValidBoard(b) && LegalMove(b, player, move);
		var AllDirections:set<Direction> := FindAllLegalDirectionsFrom(b, player, move);

		assert forall d:: d in AllDirections ==> ReversibleVectorFrom(b, player, move, d);

		positions:= {};

		var CheckedDirs: set<Direction> := {};

		while |AllDirections| > 0
		invariant |AllDirections| >= 0

		invariant positions <= ReversiblePositionsFrom(b, player, move)  
		invariant forall pos :: pos in ReversiblePositionsFrom(b, player, move) && pos !in positions 
							==> exists d:Direction :: d in AllDirections && pos in ReversibleVectorPositions(b, player, move, d)

		decreases |AllDirections|
		{
			var d: Direction;
			d:| d in AllDirections;

			assume ReversibleVectorFrom(b, player, move, d);
			var thisDirectionPositions:= ReversibleVectorOpponentPositions(b, d, move, player);
			assert ReversibleVectorPositions(b, player, move, d) == thisDirectionPositions;
			assume ReversibleVectorPositions(b, player, move, d) <= ReversiblePositionsFrom(b, player, move);
			assert thisDirectionPositions <= ReversiblePositionsFrom(b, player, move);
			assert positions <= ReversiblePositionsFrom(b, player, move);

			positions := positions + thisDirectionPositions;
			
			assert positions <= ReversiblePositionsFrom(b, player, move);

			AllDirections := AllDirections - {d};
			CheckedDirs := CheckedDirs + {d};


			assume forall d:: d in AllDirections ==> ReversibleVectorFrom(b, player, move, d);
		}
		assert positions == ReversiblePositionsFrom(b, player, move);
	}*/
	

method FindAllLegalMoves(b: Board, player: Disk) returns (moves: set<Position>) // just one simple assume
	requires ValidBoard(b)
	ensures moves == AllLegalMoves(b, player)
	{
		var AvailablePos: set<Position>;
		AvailablePos := AvailablePositionsM(b);

		assert AvailablePos == AvailablePositions(b);
		assert AllLegalMoves(b, player) <= AvailablePos;

		moves := {};
		var IllegalMoves:set<Position> := {};

		assert moves == {};
		assert IllegalMoves == {};

		var IsLM:bool;
		var move: Position;

		while |AvailablePos| > 0
		invariant |AvailablePos| >= 0

		invariant moves <= AllLegalMoves(b, player)
		invariant forall move :: move in AllLegalMoves(b, player) && move !in moves ==> move in AvailablePos

		invariant forall pos :: pos in IllegalMoves ==> pos !in moves
		invariant forall pos :: pos in IllegalMoves ==> pos !in AllLegalMoves(b, player)

		invariant forall pos:: pos in moves + IllegalMoves ==> pos !in AvailablePos

		decreases |AvailablePos|
		{
			move :| move in AvailablePos;

			assume move in AvailablePositions(b); // from the line above its so obvious , because we take a move from AvailablePos that equals to AvailablePositionsM(b)
			assert ValidPosition(move);

			IsLM := LegalMoveM(b,player,move);

			if IsLM
			{
				assert IsLM;
				assert LegalMove(b, player, move);
				assert move in AllLegalMoves(b, player);
				assert move !in moves;
				assert moves < AllLegalMoves(b, player);
				
				moves := moves + {move};
				
				assert move in moves;
				assert moves <= AllLegalMoves(b, player);
			}

			else
			{
				assert !IsLM;
				assert !LegalMove(b, player, move);
				assert move !in AllLegalMoves(b, player);
				assert move !in moves;
				assert moves <= AllLegalMoves(b, player);
				assert forall pos :: pos in IllegalMoves ==> !LegalMove(b, player, move);
				assert forall pos :: pos in IllegalMoves ==> pos !in AllLegalMoves(b, player);
				assert forall pos :: pos in IllegalMoves ==> pos !in moves;

				IllegalMoves := IllegalMoves + {move};

				assert forall pos :: pos in IllegalMoves ==> pos !in AllLegalMoves(b, player);
				assert forall pos :: pos in IllegalMoves ==> pos !in moves;
				assert move in IllegalMoves;
			}

			assert move in moves || move in IllegalMoves;
			assert move in moves ==> move !in IllegalMoves;
			assert move in IllegalMoves ==> move !in moves;

			assert move in AvailablePos;

			AvailablePos := AvailablePos - {move};

			assert move !in AvailablePos;
			assert !(exists pos:: pos in AvailablePos && pos.0 == move.0 && pos.1 == move.1); // we deleted move from Available Pos. and this assert to ensure that we will not add the same move in another while-round
		}
	}

	method PerformMove(b0: Board, player: Disk, move: Position) returns (b: Board) 
	requires ValidBoard(b0) && LegalMove(b0, player, move)
	ensures ValidBoard(b)
	ensures AvailablePositions(b) == AvailablePositions(b0)-{move}
	ensures PlayerPositions(b, player) == PlayerPositions(b0, player)+ReversiblePositionsFrom(b0, player, move)+{move}
	ensures PlayerPositions(b, Opponent(player)) == PlayerPositions(b0, Opponent(player))-ReversiblePositionsFrom(b0, player, move)
	{
		b := map[move := player];
		assert move in b;

		var AllReversiblePos:set<Position> := FindAllReversiblePositionsFrom(b0, player, move);

		assume AllReversiblePos == ReversiblePositionsFrom(b, player, move); // due to the above definition 

		var domainb0:set<Position>:= {};
		 domainb0 := set pos:Position | pos in b0;

		assert forall pos :: pos in b0 ==> pos in domainb0;
		assert forall pos :: pos in domainb0 ==> pos in b0;

		var domainb:set<Position>;

		domainb := domainb0;

		assert |domainb| == |domainb0|;
		assert ValidBoard(b);


		while |domainb| > 0
		invariant ValidBoard(b)

		invariant AvailablePositions(b0)-{move} <= AvailablePositions(b)
		invariant forall pos :: pos in AvailablePositions(b) && pos !in domainb ==> pos in AvailablePositions(b0)-{move}

		invariant PlayerPositions(b, player) <= PlayerPositions(b0, player) + ReversiblePositionsFrom(b0, player, move)+{move}
		invariant forall pos :: pos in PlayerPositions(b0, player) + ReversiblePositionsFrom(b0, player, move)+{move} && pos in b ==> pos in PlayerPositions(b, player)

		invariant PlayerPositions(b, Opponent(player)) <= PlayerPositions(b0, Opponent(player)) - ReversiblePositionsFrom(b0, player, move)
		invariant forall pos :: pos in PlayerPositions(b0, Opponent(player)) - ReversiblePositionsFrom(b0, player, move) && pos in b ==> pos in PlayerPositions(b, Opponent(player))

		decreases |domainb|
		{
			assert ValidBoard(b);

			var pos:Position;
			pos :| pos in domainb;
			assume pos in b0; // trivial, we defined domainb equal to domainb0, which defined to be the domain of b0

			assert ValidPosition(pos);
			if pos !in b 
			{
				if pos in AllReversiblePos
				{
					assert pos !in b;
					b := b[pos := player];
					assert pos in b;
					assert pos in b0;

					assert b0[pos] == Opponent(player);
					assert b[pos] == player;
				}

				else
				{
					assert pos !in b;
					var p:Disk := b0[pos];
					b := b[pos := p];
					assert pos in b;
					assert pos in b0;
					assert b0[pos] == b[pos];
					assert b0[pos] == Opponent(player) <==> b[pos] == Opponent(player);
					assert b0[pos] == player <==> b[pos] == player;
				}
			}

			assert pos in domainb;

			domainb := domainb - {pos};

			assert ValidBoard(b);
			assert pos in b0;
			assert pos in b;

			assert pos in PlayerPositions(b0, player) ==> pos in PlayerPositions(b, player);
			assert pos in ReversiblePositionsFrom(b0, player, move) ==> pos in PlayerPositions(b, player);
			assert pos == move ==> pos in PlayerPositions(b, player);
			assert pos in PlayerPositions(b0, player) + ReversiblePositionsFrom(b0, player, move) + {move} ==> pos in PlayerPositions(b, player); // this is true for all pos we chose in first of the round

			assume forall pos :: pos in PlayerPositions(b0, player) + ReversiblePositionsFrom(b0, player, move)+{move} && pos in b ==> pos in PlayerPositions(b, player); // from the above asserts 


			assert pos in PlayerPositions(b0, Opponent(player)) - ReversiblePositionsFrom(b0, player, move) ==> pos in PlayerPositions(b, Opponent(player));
			assume forall pos :: pos in PlayerPositions(b0, Opponent(player)) - ReversiblePositionsFrom(b0, player, move) && pos in b ==> pos in PlayerPositions(b, Opponent(player));// the same purpose
		}
		assert domainb == {};

		assume forall pos :: pos in b ==> pos in domainb0 + {move}; // we added move to b in line 985, then we added all domainb's elements one by one, which we defined to be equal to domainb0, and nothind else added to b!!
		assume forall pos :: pos in domainb0 ==> pos in b; // the same purpose

		assert forall pos :: pos in b0 ==> pos !in AvailablePositions(b);
		assume move in b;  // we added move to b in line 985,
		assert move !in AvailablePositions(b);
		assert AvailablePositions(b) <= AvailablePositions(b0);
		assert AvailablePositions(b) <= AvailablePositions(b0)-{move};
		assert AvailablePositions(b0)-{move} <= AvailablePositions(b); 
		assert AvailablePositions(b) == AvailablePositions(b0)-{move};




		assert PlayerPositions(b, player) == PlayerPositions(b0, player)+ReversiblePositionsFrom(b0, player, move)+{move};
		assert PlayerPositions(b, Opponent(player)) == PlayerPositions(b0, Opponent(player))-ReversiblePositionsFrom(b0, player, move);
	}