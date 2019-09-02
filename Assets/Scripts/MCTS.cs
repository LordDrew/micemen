using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MCTS
{
    public struct Move
    {
        public int MoveIndex;
        public bool Up;
    }
    class Node
    {
        public BoardState State;
        public List<Node> ChildrenUp;
        public List<int> MovesUp;
        public List<Node> ChildrenDown;
        public List<int> MovesDown;
        public int Visits;
        public float Score;
        public bool MyTurn;
        public bool IsTerminal
        {
            get => State.turnState == BoardState.TurnState.BlueVictory ||
                   State.turnState == BoardState.TurnState.RedVictory ||
                   State.turnState == BoardState.TurnState.Draw;
        }
        public bool IsFullyExpanded
        {
            get => State.validTurns.Count == ChildrenUp.Count && State.validTurns.Count == ChildrenDown.Count;
        }
    };

    Node root;
    readonly BoardState.TurnState targetState;
    public MCTS(BoardState boardState, BoardState.TurnState target)
    {
        root = new Node()
        {
            Visits = 0,
            Score = 0,
            State = new BoardState(boardState),
            ChildrenUp = new List<Node>(),
            ChildrenDown = new List<Node>(),
            MovesDown = new List<int>(),
            MovesUp = new List<int>(),
            MyTurn = true
        };
        targetState = target;
    }

    static readonly float explorationCoef = 1f;
    Node SelectNode(Node current)
    {
        Node bestChild = current;
        float bestUCT = float.MinValue;
        foreach (var node in current.ChildrenUp)
        {
            float Reward = (current.MyTurn ? 1 : -1) * node.Score / node.Visits;
            float Explore = Mathf.Sqrt(Mathf.Log10(current.Visits) / node.Visits);
            float UCT = Reward + explorationCoef * Explore;
            if (UCT > bestUCT)
            {
                bestUCT = UCT;
                bestChild = node;
            }
        }
        foreach (var node in current.ChildrenDown)
        {
            float Reward = (current.MyTurn ? 1 : -1) * node.Score / node.Visits;
            float Explore = Mathf.Sqrt(Mathf.Log10(current.Visits) / node.Visits);
            float UCT = Reward + explorationCoef * Explore;
            if (UCT > bestUCT)
            {
                bestUCT = UCT;
                bestChild = node;
            }
        }
        return bestChild;
    }

    Node ExpandNode(Node node)
    {
        var turnsUp = node.State.validTurns.Except(node.MovesUp).ToList();
        var turnsDown = node.State.validTurns.Except(node.MovesDown).ToList();
        int turn;
        bool up;
        Node newNode;
        if (turnsDown.Count > 0)
        {
            if (turnsUp.Count > 0)
            {
                up = Random.Range(0, 2) == 0;
            }
            else
            {
                up = false;
            }
        }
        else
        {
            up = true;
        }
        if (up)
        {
            turn = turnsUp[Random.Range(0, turnsUp.Count)];
            newNode = new Node()
            {
                Visits = 0,
                Score = 0,
                State = new BoardState(node.State),
                ChildrenUp = new List<Node>(),
                ChildrenDown = new List<Node>(),
                MovesDown = new List<int>(),
                MovesUp = new List<int>(),
                MyTurn = !node.MyTurn
            };
            newNode.State.MoveUp(turn);
            newNode.State.QuickMoveAllMice();
            node.ChildrenUp.Add(newNode);
            node.MovesUp.Add(turn);
        }
        else
        {
            turn = turnsDown[Random.Range(0, turnsDown.Count)];
            newNode = new Node()
            {
                Visits = 0,
                Score = 0,
                State = new BoardState(node.State),
                ChildrenUp = new List<Node>(),
                ChildrenDown = new List<Node>(),
                MovesDown = new List<int>(),
                MovesUp = new List<int>(),
                MyTurn = !node.MyTurn
            };
            newNode.State.MoveDown(turn);
            newNode.State.QuickMoveAllMice();
            node.ChildrenDown.Add(newNode);
            node.MovesDown.Add(turn);
        }
        return newNode;
    }

    public void Iterate()
    {
        List<Node> nodesToUpdate = new List<Node>();
        Node n = root;
        nodesToUpdate.Add(n);
        while (n.IsFullyExpanded && !n.IsTerminal)
        {
            n = SelectNode(n);
            nodesToUpdate.Add(n);
        }

        if (!n.IsTerminal)
        {
            n = ExpandNode(n);
            nodesToUpdate.Add(n);
        }

        var result = RandomFinish(n.State);
        for (int i = 0; i < nodesToUpdate.Count; i++)
        {
            nodesToUpdate[i].Score += result;
            nodesToUpdate[i].Visits++;
        }
    }

    public int RandomFinish(BoardState boardState)
    {
        BoardState state = new BoardState(boardState);
        for (int i = 0; i < 300; i++)
        {
            switch (state.turnState)
            {
                case BoardState.TurnState.Draw:
                    return 0;
                case BoardState.TurnState.BlueVictory:
                    return state.turnState == targetState ? 1 : -1;
                case BoardState.TurnState.RedVictory:
                    return state.turnState == targetState ? 1 : -1;
            }
            if (Random.Range(0, 2) == 0)
            {
                state.MoveUp(state.validTurns[Random.Range(0, state.validTurns.Count)]);
            }
            else
            {
                state.MoveDown(state.validTurns[Random.Range(0, state.validTurns.Count)]);
            }
            state.QuickMoveAllMice();
        }
        return 0;
    }

    public Move GetBestMove()
    {
        Move bestMove = new Move();
        int bestVisits = 0;
        int best_i = 0;
        for (int i = 0; i < root.MovesUp.Count; i++)
        {
            if (root.ChildrenUp[i].Visits > bestVisits)
            {
                bestVisits = root.ChildrenUp[i].Visits;
                bestMove.Up = true;
                bestMove.MoveIndex = root.State.validTurns.IndexOf(root.MovesUp[i]);
                best_i = i;
            }
        }
        for (int i = 0; i < root.MovesDown.Count; i++)
        {
            if (root.ChildrenDown[i].Visits > bestVisits)
            {
                bestVisits = root.ChildrenDown[i].Visits;
                bestMove.Up = false;
                bestMove.MoveIndex = root.State.validTurns.IndexOf(root.MovesDown[i]);
                best_i = i;
            }
        }
        if (bestMove.Up)
            Debug.LogFormat("Root: {0}, Best: {1} ({2})", root.Score, root.ChildrenUp[best_i].Score, bestVisits);
        else
            Debug.LogFormat("Root: {0}, Best: {1} ({2})", root.Score, root.ChildrenDown[best_i].Score, bestVisits);
        return bestMove;
    }
}
