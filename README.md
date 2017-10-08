## Neuroevolution of 2-legged creatures learning to walk further-faster

A 2D simulation using 3-layer perceptron neural networks that controls a creature's joints, updating the network's weights using simple genetic algorithms. The goal of the simulation is to evolve those creatures, that start with no idea on how to move their limbs, to learn how to walk forward and progress with each generation.

![Web demo here](http://neurothingy.azurewebsites.net/)

![Generic Screenshot](https://i.imgur.com/YmEiBZa.png)
  
 A **creature** is consisted of 2 distinct type of parts: joints and arms. This version uses creatures with 3 arms and 4 joints, from which only the 2 central joints are mechanical (can be forced to rotate). The joints have good friction with the ground, and neither part collides with another.
  
![Creature](https://i.imgur.com/xmkQdL7.png)

  
### Neural Specifics

Each creature is controled by its own neural network. The network consists of 3 layers: input-hidden-output. 

The **output** layer has 3 neurons. The first two are used as the torque input of the two mechanical joints, while the first is used as a memory and is fed back into the input.

The **input** layer has 7 neurons. The first 4 neurons use the distance from ground of each of the 4 joints as their input, while the next 2 use the current rotation angle of the mechanical joints. The last one is the memory fed from the output.

The **hidden** neurons number is defined by the user.
 
 
### Genetic Specifics

The genetic evolution can be summed up in 3 functions: Evaluation, Crossbreeding, and Mutation.

The **Evaluation** process produces a score for each creature, known as its fitness value. The value is calculated based on the horizontal distance that creature traveled, and the median speed it achieved doing so. The weights for each component are user defined in the simulation options. The evaluation happens after each generation ends. The creatures are sorted by their perfomance from highest to lowest, and the last half of them are *killed*. 

The **Crossbreeding** comes after the evaluation and is used to produce offsprings between the remaining creatures, in order to return the population to its original size for the next generation. The parents are selected with a fitness roulette, meaning that a creature with higher fitness value has more chances to be selected as a parent for reproduction. For each mating, two offsprings are generated. A random hidden neuron is selected at first (the same on both parents), then each child is created as a copy of one parent and has the selected neuron weights swaped by the correspondent weights of the other parent's same neuron.

Lastly, the **Mutation** is used to insert more randomness to the popuplation, to trigger evolutions faster. The process selects a random number of weights and changes their value. There are 4 types of mutations: 
* Random value, which selects a completely new random value for the weight.
* Scaling the old value by a random factor
* Adding/Substracting a random constant from the weight
* Changing the weights sign to the opposite one (multiplying with -1)

![Simulation Options](https://i.imgur.com/eaosCGA.png)
