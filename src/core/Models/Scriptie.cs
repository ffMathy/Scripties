using Akka.Persistence;

namespace Scripties.Core.Models
{
    public class Scriptie : UntypedPersistentActor
    {
        public override string PersistenceId { get; }
        
        public Position Position { get; private set; }
        
        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case MoveCommand moveCommand:
                    Persist(
                        new PositionChangedEvent(
                            new Position(
                                Position.X + (moveCommand.Direction switch
                                {
                                    Direction.Left or Direction.LowerLeft or Direction.UpperLeft => -1,
                                    Direction.Right or Direction.LowerRight or Direction.UpperRight => 1,
                                    _ => 0
                                }),
                                Position.Y + (moveCommand.Direction switch
                                {
                                    Direction.Up or Direction.UpperLeft or Direction.UpperRight => -1,
                                    Direction.Down or Direction.LowerLeft or Direction.LowerRight => 1,
                                    _ => 0
                                }))),
                        e => Position = e.NewPosition);
                    return;
            }
            
            throw new System.NotImplementedException();
        }

        protected override void OnRecover(object message)
        {
            throw new System.NotImplementedException();
        }

        private class PositionChangedEvent
        {
            public PositionChangedEvent(Position newPosition)
            {
                NewPosition = newPosition;
            }

            public Position NewPosition { get; }
        }

        private class MoveCommand
        {
            public MoveCommand(Direction direction)
            {
                Direction = direction;
            }

            public Direction Direction { get; }
        }
    }
}