import matplotlib.pyplot as plt
import pandas as pd
import sys

if len(sys.argv) < 2:
    print('Data file not specified')
    quit()

filename = sys.argv[1]

df = pd.read_csv(filename, sep=';', decimal=',', header=None, index_col=0)
df.plot.scatter(x=2, y=3, c=1, cmap='tab20')
plt.xlim(-0.025, 0.025);

plt.title("This is your Clasterization")
plt.show()