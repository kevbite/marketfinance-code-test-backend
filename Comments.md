# Code Issues with ProductApplicationService
1. Everything is in one method, even if you split it out in to separate methods the class would have low cohesion. Ideally we'd go for some sort of strategy pattern to eliminate the low cohesion and branching.
2. Branching and early returns makes it hard to read the control flow, same as above we could abstract these away.
3. The modeling of `IProduct` seems a little odd, we could be using interfaces on top of the objects to stop modifications to the underlining objects also the ProductApplicationService only uses the getters, that way we'd adhere to the interface segregation principle. However, modifying this would be a breaking change to the rest of the code relaying on these interfaces.
4. ToStringing a integer could cause problems in the future even if it works currently now even if you don't have the culture changing per execution. The ToString method with no arguments uses the current culture to format the number, cultures often change over time for many reasons. To fix this and make it stable for the future we could pass in a the invariant culture.
5. Duplication of code when creating `CompanyDataRequest`
 