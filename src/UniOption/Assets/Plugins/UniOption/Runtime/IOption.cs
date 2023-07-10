using UnityEngine;

namespace UniOption {
    public interface IOption {
        public bool IsSome { get; }
        public bool IsNone { get; }
    }
}