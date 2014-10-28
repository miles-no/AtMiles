@miles
=======

@miles' purpose in this world is to be a playground to learn and/or try out new technologies and techniques. 

@miles has a small set of use-cases around managing employees in a company. First and foremost as a contact-list. Available for web, IPhone and Android.
More features and platforms might sudden appear.

Disclaimer:
Some of the stuff you will see here is WAY over the top. Some call it architecural goldplating, or "stealing from the business". We call it learning.
However, this project gets it's commits from consultants between clients, or in their spare time. So no business to steal from.
We allow ourself to to "all-out" on ideas, to learn and master techniques, to later be used in bigger projects.

The only thing we regret is the fact that this problem/domain is not a perfect match for CQRS/ES. In almost every situation, this is a typical CRUD-domain, and should be treated that way. The domain lacks the typical concurrent actions that is found in good canditate for CQRS.
However an event driven architecture is almost never wrong.
We decided to bend this rule of "best practice" to test out some infrastructure stuff, and in general get some experience with CQRS and EventSourcing.
